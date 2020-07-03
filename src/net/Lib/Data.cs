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
using Azure.Security.KeyVault.Secrets;
using Azure.AI.FormRecognizer;
using Azure.AI.TextAnalytics;
using DotNetEnv;

namespace Lib
{
    public class Data
    {
        public CosmosClient CosmosClient;
        public CosmosContainer CosmosContainer;
        public SecretClient SecretClient;
        public ChainedTokenCredential credential = Identity.GetCredentialChain();
        public BlobServiceClient BlobServiceClient;
        public BlobContainerClient ContainerClient;
        public QueueServiceClient QueueServiceClient;
        public QueueClient QueueClient;
        public TextAnalyticsClient TextAnalyticsClient;
        public FormRecognizerClient FormRecognizerClient;

        public Data()
        {
        }

        public async Task InitializeAsync()
        {
            // KeyVault
            SecretClient = new SecretClient(new Uri(Env.GetString("AZURE_KEYVAULT_ENDPOINT")), credential);
            var cosmosKey = await SecretClient.GetSecretAsync(Env.GetString("AZURE_COSMOS_KEY_NAME", "cosmoskey"));

            // Cosmos
            CosmosClient = new CosmosClient(
                            Env.GetString("AZURE_COSMOS_ENDPOINT"),
                            cosmosKey.Value.Value,
                            new CosmosClientOptions
                            {
                                ConnectionMode = ConnectionMode.Direct,
                                ConsistencyLevel = ConsistencyLevel.Session
                            });

            CosmosContainer = CosmosClient.GetDatabase(Env.GetString("AZURE_COSMOS_DB")).GetContainer(Env.GetString("AZURE_COSMOS_CONTAINER"));

            // Blob
            BlobServiceClient = new BlobServiceClient(new Uri(Env.GetString("AZURE_STORAGE_BLOB_ENDPOINT")), credential);
            ContainerClient = BlobServiceClient.GetBlobContainerClient(Env.GetString("AZURE_STORAGE_BLOB_CONTAINER_NAME"));
            await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            // Queue
            QueueServiceClient = new QueueServiceClient(new Uri(Env.GetString("AZURE_STORAGE_QUEUE_ENDPOINT")), credential);
            QueueClient = QueueServiceClient.GetQueueClient(Env.GetString("AZURE_STORAGE_QUEUE_NAME"));
            await QueueClient.CreateIfNotExistsAsync();

            // FormRecognizerClient
            FormRecognizerClient = new FormRecognizerClient(
                new Uri(Env.GetString("AZURE_FORM_RECOGNIZER_ENDPOINT")),
                credential);

            // TextAnalyticsClient
            TextAnalyticsClient = new TextAnalyticsClient(
                new Uri(Env.GetString("AZURE_TEXT_ANALYTICS_ENDPOINT")),
                credential);
        }

        public async Task<Image> EnqueueImageAsync(Image image = null)
        {
            using var http = new HttpClient();

            if (image?.Url is null || string.IsNullOrEmpty(image.Url))
            {
                image = await http.GetFromJsonAsync<Image>(Env.GetString("MEME_ENDPOINT"));
            }

            // Get Image Stream
            using var imageStream = await http.GetStreamAsync(image.Url);

            // Upload to Blob
            var blobClient = ContainerClient.GetBlobClient(image.BlobName);
            await blobClient.UploadAsync(imageStream);

            Console.WriteLine($"Uploaded to Blob Storage: {blobClient.Uri}");

            image.BlobUri = blobClient.Uri.ToString();

            // Send Queue Message
            var sendReceipt = await QueueClient.SendMessageAsync(
                JsonSerializer.Serialize<Image>(image,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase })
                );

            Console.WriteLine($"Added to Queue: {sendReceipt.Value.MessageId}");
            return image;
        }

        public async IAsyncEnumerable<Image> GetImagesAsync()
        {
            await foreach (var item in CosmosContainer.GetItemQueryIterator<Image>("SELECT * FROM c F ORDER BY F.createdDate DESC"))
            {
                yield return item;
            }
        }

        public async Task<Image> GetImageAsync(string id)
        {
            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c F WHERE F.id = @id").WithParameter("@id", id);

            await foreach (Image item in CosmosContainer.GetItemQueryIterator<Image>(queryDefinition))
            {
                return item;
            }
            return null;
        }
    }
}
