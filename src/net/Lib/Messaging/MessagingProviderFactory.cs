using System;
using Azure.Storage.Queues.Models;
using Lib.Messaging.Providers;

namespace Lib.Messaging
{
    public static class MessagingProviderFactory
    {
        public static IMessagingProvider Get(string type)
        {
            switch (type)
            {
                case "STORAGE_QUEUE":
                    return new StorageQueueMessagingProvider();
                case "SERVICE_BUS_QUEUE":
                    return new ServiceBusQueueMessagingProvider();
                default:
                    throw new Exception("MESSAGING_TYPE env var not set.");
            }
        }
    }
}