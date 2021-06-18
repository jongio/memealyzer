using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure.AI.FormRecognizer;
using Azure.AI.TextAnalytics;
using Azure.Data.AppConfiguration;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Lib.Configuration;
using Lib.Data;
using Lib.Ingest;
using Lib.Media;
using Lib.Messaging;
using Lib.Storage;

namespace Lib
{
    public class Clients : IAsyncDisposable
    {
        public ChainedTokenCredential credential = Identity.GetCredentialChain();

        public SecretClient SecretClient;
        public IStorageClient StorageClient;
        public IMessagingProvider MessagingProvider;
        public TextAnalyticsClient TextAnalyticsClient;
        public FormRecognizerClient FormRecognizerClient;
        public ConfigurationClient ConfigurationClient;
        public IDataProvider DataProvider;
        public IImageClient ImageClient;
        public IIngestClient IngestClient;
        public HttpClient HttpClient = new HttpClient();

        public Clients()
        {
        }

        public async Task InitializeAsync()
        {
            // Data Provider            
            DataProvider = DataProviderFactory.Get(Config.StorageType);
            await DataProvider.InitializeAsync(credential);

            // Messaging Provider            
            MessagingProvider = MessagingProviderFactory.Get(Config.MessagingType);
            await MessagingProvider.InitializeAsync(credential, DataProvider);

            // App Config
            ConfigurationClient = new ConfigurationClient(Config.AppConfigEndpoint, credential);

            // Storage
            StorageClient = new StorageClient();
            await StorageClient.InitializeAsync(credential);

            // FormRecognizerClient
            FormRecognizerClient = new FormRecognizerClient(Config.FormRecognizerEndpoint, credential);

            // TextAnalyticsClient
            TextAnalyticsClient = new TextAnalyticsClient(Config.TextAnalyticsEndpoint, credential);

            // ImageClient
            ImageClient = new ImageClient(HttpClient);

            // IngestClient
            IngestClient = new IngestClient(ImageClient, StorageClient, MessagingProvider);
        }

        public ValueTask DisposeAsync()
        {
            if (DataProvider != null)
            {
                DataProvider.Dispose();
            }
            
            if (MessagingProvider != null)
            {
                return MessagingProvider.DisposeAsync();
            }
            else
            {
                return new ValueTask();
            }
        }
    }
}
