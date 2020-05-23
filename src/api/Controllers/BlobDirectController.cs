
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Identity;
using System;

namespace azsdkdemoapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlobDirectController : ControllerBase
    {

        public BlobDirectController()
        {
        }

        // GET: api/Blob
        [HttpGet]
        public async Task<object> Get()
        {
            // Create a container in our Storage account:
            var serviceUri = new Uri(Environment.GetEnvironmentVariable("AZURE_STORAGE_BLOB_URI"));
            var serviceClient = new BlobServiceClient(serviceUri, new DefaultAzureCredential());
            
            var containerClient = serviceClient.GetBlobContainerClient("blobs");
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Upload a blob to our container:
            var blobClient = containerClient.GetBlobClient("blob.txt");
            var blob = await blobClient.UploadAsync(new MemoryStream(Encoding.UTF8.GetBytes("Click here to view the latest Azure SDK releases: https://aka.ms/azsdk/releases")), overwrite: true);

            // Return the blob contents:
            var blobDownload = await blobClient.DownloadAsync();
            
            using var blobStreamReader = new StreamReader(blobDownload.Value.Content);

            return new {Content = await blobStreamReader.ReadToEndAsync()};
        }
    }
}
