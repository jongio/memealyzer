
using Lib.Model;

namespace Lib.Messaging
{
    public class ImageQueueMessage
    {
        public Message Message { get; set; }
        public IImage Image { get; set; }
        public object NativeMessage { get; set; }
    }
}
