using System;
using Azure.Messaging.ServiceBus;

namespace Lib.Messaging.Providers
{
    public class ServiceBusQueueMessage : IMessage
    {
        ServiceBusReceivedMessage Message;

        public string Id => this.Message.MessageId;
        public string Receipt => this.Message.LockToken;
        public string Text => this.Message.Body.ToString();

        public ServiceBusQueueMessage(ServiceBusReceivedMessage message)
        {
            Message = message;
        }

        public T GetNativeMessage<T>() where T : class
        {
            if (typeof(T) != typeof(ServiceBusReceivedMessage))
            {
                throw new InvalidOperationException();
            }
            return this.Message as T;
        }
    }
}