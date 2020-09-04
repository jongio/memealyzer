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

            DataProvider = new DataProviderFactory().GetDataProvider(Env.GetString("AZURE_STORAGE_TYPE"));
            await DataProvider.InitializeAsync(credential);

            // App Config
            ConfigurationClient = new ConfigurationClient(new Uri(Env.GetString("AZURE_APP_CONFIG_ENDPOINT")), credential);

            // Blob
            BlobServiceClient = new BlobServiceClient(new Uri(Env.GetString("AZURE_STORAGE_BLOB_ENDPOINT")), credential);
            ContainerClient = BlobServiceClient.GetBlobContainerClient(Env.GetString("AZURE_STORAGE_BLOB_CONTAINER_NAME"));
            await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            // Queue
            QueueServiceClient = new QueueServiceClient(new Uri(Env.GetString("AZURE_STORAGE_QUEUE_ENDPOINT")), credential);
            QueueClient = QueueServiceClient.GetQueueClient(Env.GetString("AZURE_STORAGE_QUEUE_NAME"));
            await QueueClient.CreateIfNotExistsAsync();

            // Client Sync Queue
            ClientSyncQueueClient = QueueServiceClient.GetQueueClient(Env.GetString("AZURE_STORAGE_CLIENT_SYNC_QUEUE_NAME"));
            await ClientSyncQueueClient.CreateIfNotExistsAsync();

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
            if (image?.Url is null || string.IsNullOrEmpty(image.Url))
            {
                var memeImage = await httpClient.GetFromJsonAsync<Image>(Env.GetString("MEME_ENDPOINT"));
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
