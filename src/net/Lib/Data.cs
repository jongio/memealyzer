using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Identity;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Queues;
using Azure.Cosmos;
using Azure.Cosmos.Serialization;
using DotNetEnv;

namespace Lib
{
    public static class Data
    {
        public async static Task<Image> EnqueueImageAsync(Image image = null)
        {
            using var http = new HttpClient();

            if (image?.Url is null || string.IsNullOrEmpty(image.Url))
            {
                image = await http.GetFromJsonAsync<Image>(Env.GetString("MEME_ENDPOINT"));
            }

            var cred = Identity.GetCredentialChain();

            // BlobServiceClient
            var blobServiceClient = new BlobServiceClient(new Uri(Env.GetString("AZURE_STORAGE_BLOB_ENDPOINT")), cred);
            var containerClient = blobServiceClient.GetBlobContainerClient(Env.GetString("AZURE_STORAGE_BLOB_CONTAINER_NAME"));

            //TODO: Move the container creation code to startup because it takes a while and we don't want to do on every request.
            //await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            // Get Image Stream
            using var imageStream = await http.GetStreamAsync(image.Url);

            // Upload to Blob
            var blobClient = containerClient.GetBlobClient(image.BlobName);
            await blobClient.UploadAsync(imageStream);

            Console.WriteLine($"Uploaded to Blob Storage: {blobClient.Uri}");

            image.BlobUri = blobClient.Uri.ToString();

            // QueueClient
            var queueServiceClient = new QueueServiceClient(
                new Uri(Env.GetString("AZURE_STORAGE_QUEUE_ENDPOINT")), cred);

            var queueClient = queueServiceClient.GetQueueClient(Env.GetString("AZURE_STORAGE_QUEUE_NAME"));

            //TODO: Move the queue creation code to startup because it takes a while and we don't want to do on every request.
            //await queueClient.CreateIfNotExistsAsync();

            // Send Queue Message
            var sendReceipt = await queueClient.SendMessageAsync(
                JsonSerializer.Serialize<Image>(image,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                );

            Console.WriteLine($"Added to Queue: {sendReceipt.Value.MessageId}");
            return image;
        }

        public async static IAsyncEnumerable<Image> GetImagesAsync()
        {
            var cosmosClient = new CosmosClient(
                Env.GetString("AZURE_COSMOS_ENDPOINT"),
                Env.GetString("AZURE_COSMOS_KEY"),
                new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConsistencyLevel = ConsistencyLevel.Session
                });

            var cosmosContainer = cosmosClient.GetDatabase(Env.GetString("AZURE_COSMOS_DB")).GetContainer(Env.GetString("AZURE_COSMOS_CONTAINER"));

            await foreach (var item in cosmosContainer.GetItemQueryIterator<Image>("SELECT * FROM c F ORDER BY F.createdDate DESC"))
            {
                yield return item;
            }
        }

        public async static Task<Image> GetImageAsync(string id)
        {
            var cosmosClient = new CosmosClient(
                Env.GetString("AZURE_COSMOS_ENDPOINT"),
                Env.GetString("AZURE_COSMOS_KEY"),
                new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConsistencyLevel = ConsistencyLevel.Session
                });

            var cosmosContainer = cosmosClient.GetDatabase(Env.GetString("AZURE_COSMOS_DB")).GetContainer(Env.GetString("AZURE_COSMOS_CONTAINER"));

            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c F WHERE F.id = @id").WithParameter("@id", id);

            await foreach (Image item in cosmosContainer.GetItemQueryIterator<Image>(queryDefinition))
            {
                return item;
            }
            return null;
        }
    }
}
