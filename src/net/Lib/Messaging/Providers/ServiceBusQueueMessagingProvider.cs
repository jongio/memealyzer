using System;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Messaging.ServiceBus;
using Lib.Data;

namespace Lib.Messaging.Providers
{
    public class ServiceBusQueueMessagingProvider : IMessagingProvider
    {
        ServiceBusClient ServiceBusClient { get; set; }
        public IQueue ImageQueueClient { get; set; }
        public IQueue ClientSyncQueueClient { get; set; }

        public async Task InitializeAsync(TokenCredential credential, IDataProvider dataProvider)
        {
            ServiceBusClient = new ServiceBusClient(Config.ServiceBusNamespace, credential);

            ImageQueueClient = new ServiceBusQueue(ServiceBusClient, Config.MessagesQueueName, dataProvider);
            await ImageQueueClient.InitializeAsync();

            ClientSyncQueueClient = new ServiceBusQueue(ServiceBusClient, Config.ClientSyncQueueName, dataProvider);
            await ClientSyncQueueClient.InitializeAsync();
        }

        public ValueTask DisposeAsync()
        {
            return ServiceBusClient.DisposeAsync();
        }
    }
}