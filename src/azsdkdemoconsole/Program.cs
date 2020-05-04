using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DotNetEnv;

namespace azsdkdemoconsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Env.Load("../../.env");

            // Set blob client options for more retries:
            var options = new ClientOptions()
            {
                Retry = {
                    MaxRetries = 10,
                    Delay = TimeSpan.FromSeconds(3)
                },
                Diagnostics = {
                    IsLoggingEnabled = false
                }
            };

            // Add our own custom policy to the HTTP pipeline:
            options.AddPolicy(new SimpleTracingPolicy(), HttpPipelinePosition.PerCall);

            // Get storage account blob uri from environment
            var blobServiceUri = new Uri(Environment.GetEnvironmentVariable("AZURE_STORAGE_BLOB_URI"));

            // Create a BlobServiceClient to our Storage account using DefaultAzureCredentials & our HTTP pipeline options:
            var serviceClient = new BlobServiceClient(blobServiceUri, 
                new DefaultAzureCredential(), options);
            
            // Create a BlobContainerClient
            var containerClient = serviceClient.GetBlobContainerClient("blobs");

            // Create a container in our Storage account:
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.Blob);

            // Get the blob client:
            var blobClient = containerClient.GetBlobClient("blob.txt");

            // Upload a blob to container:
            var blob = await blobClient.UploadAsync(
                new MemoryStream(Encoding.UTF8.GetBytes("Click here to view the latest Azure SDK releases: https://aka.ms/azsdk/releases")),
                overwrite: true);

            // Download the blob
            var blobDownload = await blobClient.DownloadAsync();
            using var blobStreamReader = new StreamReader(blobDownload.Value.Content);

            // Write the blob contents
            Console.WriteLine($"Content: {blobStreamReader.ReadToEnd()}");

            Console.Read();
        }
    }

    public class SimpleTracingPolicy : HttpPipelinePolicy
    {
        public override async ValueTask ProcessAsync(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            Console.WriteLine($">> Request: {message.Request.Method} {message.Request.Uri}");
            await ProcessNextAsync(message, pipeline);
            Console.WriteLine($">> Response: {message.Response.Status} from {message.Request.Method} {message.Request.Uri}\n");
        }

        #region public override void Process
        public override void Process(HttpMessage message, ReadOnlyMemory<HttpPipelinePolicy> pipeline)
        {
            Console.WriteLine($">> Request: {message.Request.Uri}");
            ProcessNext(message, pipeline);
            Console.WriteLine($">> Response: {message.Response.Status} {message.Request.Method} {message.Request.Uri}");
        }
        #endregion public override void Process
    }
}
