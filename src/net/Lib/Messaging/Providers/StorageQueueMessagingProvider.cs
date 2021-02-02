using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Queues;
using Lib.Data;

namespace Lib.Messaging.Providers
{
    public class StorageQueueMessagingProvider : IMessagingProvider
    {
        public IQueue ImageQueueClient { get; set; }
        public IQueue ClientSyncQueueClient { get; set; }

        public ValueTask DisposeAsync()
        {
            return new ValueTask();
        }

        public async Task InitializeAsync(TokenCredential credential, IDataProvider dataProvider) 
        {
            var queueServiceClient = new QueueServiceClient(Config.StorageQueueEndpoint, credential);

            ImageQueueClient = new StorageQueue(queueServiceClient, Config.MessagesQueueName, dataProvider);
            await ImageQueueClient.InitializeAsync();

            ClientSyncQueueClient = new StorageQueue(queueServiceClient, Config.ClientSyncQueueName, dataProvider);
            await ClientSyncQueueClient.InitializeAsync();
        }
    }
}