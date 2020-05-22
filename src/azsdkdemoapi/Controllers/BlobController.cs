
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;

using Microsoft.Extensions.Azure;
using System;

namespace azsdkdemoapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobController : ControllerBase
    {
        private readonly BlobServiceClient blobServiceClient;
        private readonly QueueServiceClient queueServiceClient;


        public BlobController(BlobServiceClient blobServiceClient, QueueServiceClient queueServiceClient)
        {
            this.blobServiceClient = blobServiceClient;
            this.queueServiceClient = queueServiceClient;
        }

        // GET: api/Blob
        [HttpGet]
        public async Task<object> Get()
        {
            // Create a container in our Storage account:
            var containerClient = this.blobServiceClient.GetBlobContainerClient(Environment.GetEnvironmentVariable("AZURE_STORAGE_BLOB_NAME"));
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Upload a blob to our container:
            var blobClient = containerClient.GetBlobClient("blob.txt");

            var blob = await blobClient.UploadAsync(
                new MemoryStream(
                    Encoding.UTF8.GetBytes("Click here to view the latest Azure SDK releases: https://aka.ms/azsdk/releases")),
                    overwrite: true);

            // Create a queue if it doesn't exist
            var queueClient = queueServiceClient.GetQueueClient(Environment.GetEnvironmentVariable("AZURE_STORAGE_QUEUE_NAME")); 
            await queueClient.CreateIfNotExistsAsync();
            
            // Send msg to Queue
            var sendReceipt = await queueClient.SendMessageAsync(blobClient.Uri.ToString());

            // Return the blob contents
            var blobDownload = await blobClient.DownloadAsync();

            using var blobStreamReader = new StreamReader(blobDownload.Value.Content);

            return new { Content = blobStreamReader.ReadToEnd() };
        }
    }
}
