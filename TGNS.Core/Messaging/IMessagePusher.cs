namespace TGNS.Core.Messaging
{
    public interface IMessagePusher
    {
        PushSummary Push(string channelId, string title, string message);
        string PlatformName { get; }
        bool WasSuccessful(PushSummary summary);
    }
}