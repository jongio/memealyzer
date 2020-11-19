using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Lib.Data;

namespace Lib.Messaging.Providers
{
    public class ServiceBusQueueMessagingProvider : IMessagingProvider
    {
        public IQueue ImageQueueClient { get; set; }
        public IQueue ClientSyncQueueClient { get; set; }

        public ServiceBusQueueMessagingProvider()
        {
        }

        public async Task InitializeAsync(TokenCredential credential, IDataProvider dataProvider)
        {
            await Task.Run(() =>
            {
                var serviceBusQueueClient = new ServiceBusClient(Config.ServiceBusNamespace, credential);
                ImageQueueClient = new ServiceBusQueue(serviceBusQueueClient, Config.StorageQueueName, dataProvider);
                ClientSyncQueueClient = new ServiceBusQueue(serviceBusQueueClient, Config.StorageClientSyncQueueName, dataProvider);
            });
        }
    }
}