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
using Azure.Security.KeyVault.Secrets;
using Azure.AI.FormRecognizer;
using Azure.AI.TextAnalytics;
using Azure.Data.AppConfiguration;
using DotNetEnv;
using Lib.Data;
using Lib.Model;

namespace Lib
{
    public class Clients
    {
        public SecretClient SecretClient;
        public ChainedTokenCredential credential = Identity.GetCredentialChain();
        public BlobServiceClient BlobServiceClient;
        public BlobContainerClient ContainerClient;
        public QueueServiceClient QueueServiceClient;
        public QueueClient QueueClient;
        public QueueClient ClientSyncQueueClient;
        public TextAnalyticsClient TextAnalyticsClient;
        public FormRecognizerClient FormRecognizerClient;
        public ConfigurationClient ConfigurationClient;
        public IDataProvider DataProvider;
        private HttpClient httpClient = new HttpClient();

        public Clients()
        {
        }

        public async Task InitializeAsync()
        {
            // Data Provider            
            DataProvider = new DataProviderFactory().GetDataProvider(Config.StorageType);
            await DataProvider.InitializeAsync(credential);

            // App Config
            ConfigurationClient = new ConfigurationClient(Config.AppConfigEndpoint, credential);

            // Blob
            BlobServiceClient = new BlobServiceClient(Config.StorageBlobEndpoint, credential);
            ContainerClient = BlobServiceClient.GetBlobContainerClient(Config.StorageBlobContainerName);
            await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            // Queue
            QueueServiceClient = new QueueServiceClient(Config.StorageQueueEndpoint, credential);
            QueueClient = QueueServiceClient.GetQueueClient(Config.StorageQueueName);
            await QueueClient.CreateIfNotExistsAsync();

            // Client Sync Queue
            ClientSyncQueueClient = QueueServiceClient.GetQueueClient(Config.StorageClientSyncQueueName);
            await ClientSyncQueueClient.CreateIfNotExistsAsync();

            // FormRecognizerClient
            FormRecognizerClient = new FormRecognizerClient(Config.FormRecognizerEndpoint, credential);

            // TextAnalyticsClient
            TextAnalyticsClient = new TextAnalyticsClient(Config.TextAnalyticsEndpoint, credential);
        }

        public async Task<Image> EnqueueImageAsync(Image image = null)
        {
            if (image?.Url is null || string.IsNullOrEmpty(image.Url))
            {
                var memeImage = await httpClient.GetFromJsonAsync<Image>(Config.MemeEndpoint);
                image.Url = memeImage.Url;
            }

            // Get Image Stream
            using var imageStream = await httpClient.GetStreamAsync(image.Url);

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
    }
}
