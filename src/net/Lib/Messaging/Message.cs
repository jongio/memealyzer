using System;

namespace Lib.Messaging
{
    public class Message : IMessage
    {
        public string Id {get;set;}
        public string Receipt {get;set;}
        public string Text {get;set;}

        public Message()
        {
        }

        public T GetNativeMessage<T>() where T : class
        {
            throw new NotImplementedException();
        }
    }
}
