using System.Collections.Generic;
using System.Threading.Tasks;

namespace Lib.Messaging
{
    public interface IQueue
    {
        Task InitializeAsync();
        Task<List<ImageQueueMessage>> ReceiveMessagesAsync();
        Task<ImageQueueMessage> DeleteMessageAsync(ImageQueueMessage imageQueueMessage);
        Task<ImageQueueMessage> SendMessageAsync(ImageQueueMessage imageQueueMessage, bool encode = false);
    }
}