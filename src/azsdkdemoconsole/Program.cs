using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using DotNetEnv;

namespace azsdkdemoconsole
{
    partial class Program
    {
        static async Task Main(string[] args)
        {
            // Set blob client options for more retries:
            var options = new BlobClientOptions()
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

              
        }

        static Program()
        {
            Env.Load("../../.env");
        }
    }
}
