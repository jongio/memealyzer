using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Queues;
using Lib.Data;
using Memealyzer;

namespace Lib.Messaging.Providers
{
    public class StorageQueueMessagingProvider : IMessagingProvider
    {
        public IQueue ImageQueueClient { get; set; }
        public IQueue ClientSyncQueueClient { get; set; }

        public async Task InitializeAsync(TokenCredential credential, IDataProvider dataProvider) 
        {
            var queueServiceClient = Config.UseAzurite ?
                new QueueServiceClient(Config.AzuriteConnectionString) :
                new QueueServiceClient(Config.StorageQueueEndpoint, credential);

            ImageQueueClient = new StorageQueue(queueServiceClient, Config.MessagesQueueName, dataProvider);
            await ImageQueueClient.InitializeAsync();

            ClientSyncQueueClient = new StorageQueue(queueServiceClient, Config.ClientSyncQueueName, dataProvider);
            await ClientSyncQueueClient.InitializeAsync();
        }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }
    }
}