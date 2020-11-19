using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Storage.Queues;
using Lib.Data;
using Lib.Model;


namespace Lib.Messaging.Providers
{
    public class StorageQueue : IQueue
    {
        QueueClient queueClient;
        IDataProvider dataProvider;

        public StorageQueue(QueueClient queueClient, IDataProvider dataProvider)
        {
            this.queueClient = queueClient;
            this.dataProvider = dataProvider;
        }

        public async Task<List<ImageQueueMessage>> ReceiveMessagesAsync()
        {
            var messages = await this.queueClient.ReceiveMessagesAsync(maxMessages: Config.StorageQueueMessageCount);
            var imageQueueMessages = new List<ImageQueueMessage>();
            foreach (var message in messages.Value)
            {
                imageQueueMessages.Add(new ImageQueueMessage
                {
                    Message = new Message
                    {
                        Id = message.MessageId,
                        Receipt = message.PopReceipt,
                        Text = message.MessageText,
                    },
                    Image = dataProvider.DeserializeImage(message.MessageText),
                    NativeMessage = message
                });
            }
            return imageQueueMessages;
        }

        public async Task<ImageQueueMessage> DeleteMessageAsync(ImageQueueMessage imageQueueMessage)
        {
            await this.queueClient.DeleteMessageAsync(imageQueueMessage.Message.Id, imageQueueMessage.Message.Receipt);
            return imageQueueMessage;
        }

        public async Task<ImageQueueMessage> SendMessageAsync(ImageQueueMessage imageQueueMessage, bool encode = false)
        {
            var text = JsonSerializer.Serialize<IImage>(
                            imageQueueMessage.Image,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                        );

            if (encode)
            {
                text = Convert.ToBase64String(Encoding.UTF8.GetBytes(text));

            }

            var response = await this.queueClient.SendMessageAsync(text);

            imageQueueMessage.Message = new Message { Id = response.Value.MessageId, Receipt = response.Value.PopReceipt };
            return imageQueueMessage;
        }
    }
}