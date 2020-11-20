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

        public async Task InitializeAsync(TokenCredential credential, IDataProvider dataProvider)
        {
            // Queue
            var queueServiceClient = new QueueServiceClient(Config.StorageQueueEndpoint, credential);
            var queueClient = queueServiceClient.GetQueueClient(Config.StorageQueueName);
            await queueClient.CreateIfNotExistsAsync();

            ImageQueueClient = new StorageQueue(queueClient, dataProvider);

            // Client Sync Queue
            var clientSyncQueueClient = queueServiceClient.GetQueueClient(Config.StorageClientSyncQueueName);
            await clientSyncQueueClient.CreateIfNotExistsAsync();

            ClientSyncQueueClient = new StorageQueue(clientSyncQueueClient, dataProvider);
        }
    }
}