namespace Lib.Messaging
{
    public interface IMessage
    {
        string Id { get; }
        string Receipt { get; }
        string Text { get; }
        T GetNativeMessage<T>() where T : class;
    }
}