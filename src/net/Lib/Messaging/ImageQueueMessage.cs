
using Lib.Model;

namespace Lib.Messaging
{
    public class ImageQueueMessage
    {
        public IMessage Message { get; set; }
        public IImage Image { get; set; }
    }
}