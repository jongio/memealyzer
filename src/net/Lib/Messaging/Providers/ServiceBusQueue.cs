using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Lib.Data;
using Lib.Model;

namespace Lib.Messaging.Providers
{
    public class ServiceBusQueue : IQueue
    {
        ServiceBusClient client;
        ServiceBusSender sender;
        ServiceBusReceiver receiver;
        string queue;
        IDataProvider dataProvider;

        public ServiceBusQueue(ServiceBusClient client, string queue, IDataProvider dataProvider)
        {
            this.client = client;
            this.queue = queue;
            this.dataProvider = dataProvider;

            this.sender = this.client.CreateSender(this.queue);
            this.receiver = this.client.CreateReceiver(this.queue);
        }

        public async Task<List<ImageQueueMessage>> ReceiveMessagesAsync()
        {
            var messages = await this.receiver.ReceiveMessagesAsync(maxMessages: Config.StorageQueueMessageCount);

            var imageQueueMessages = new List<ImageQueueMessage>();
            foreach (var message in messages)
            {
                imageQueueMessages.Add(new ImageQueueMessage
                {
                    Message = new Message
                    {
                        Id = message.MessageId,
                        Receipt = message.LockToken,
                        Text = message.Body.ToString()
                    },
                    Image = dataProvider.DeserializeImage(message.Body.ToString()),
                    NativeMessage = message
                });
            }
            return imageQueueMessages;
        }

        public async Task<ImageQueueMessage> DeleteMessageAsync(ImageQueueMessage imageQueueMessage)
        {
            await this.receiver.CompleteMessageAsync((ServiceBusReceivedMessage)imageQueueMessage.NativeMessage);
            return imageQueueMessage;
        }

        public async Task<ImageQueueMessage> SendMessageAsync(ImageQueueMessage imageQueueMessage, bool encode = false)
        {
            var text = JsonSerializer.Serialize<IImage>(
                            imageQueueMessage.Image,
                            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
                        );

            await this.sender.SendMessageAsync(new ServiceBusMessage(text));
            return imageQueueMessage;
        }
    }
}