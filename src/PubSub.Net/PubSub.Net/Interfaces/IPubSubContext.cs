using System.Reflection;

namespace PubSub.Net.Interfaces
{
    public interface IPubSubContext<TEvent>
        where TEvent : class, IEventBase
    {
        TEvent Event { get; }

        TypeInfo MessageType { get; }

        string SubscriptionName { get; }

        Dictionary<string, string> Metadata { get; }
    }
}
