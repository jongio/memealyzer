using System;
using Azure.Storage.Queues.Models;

namespace Lib.Messaging
{
    public class StorageQueueMessage : IMessage
    {
        QueueMessage Message;

        public string Id => this.Message.MessageId;
        public string Receipt => this.Message.PopReceipt;
        public string Text => this.Message.MessageText;

        public StorageQueueMessage(QueueMessage message)
        {
            Message = message;
        }

        public T GetNativeMessage<T>() where T : class
        {
            if (typeof(T) != typeof(QueueMessage))
            {
                throw new InvalidOperationException();
            }
            return this.Message as T;
        }
    }
}