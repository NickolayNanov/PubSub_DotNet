using PubSub.Net.Interfaces;
using System.Reflection;

namespace PubSub.Net
{
    public class PubSubContext<TEvent> : IPubSubContext<TEvent>
        where TEvent : class, IEventBase
    {
        public PubSubContext(TEvent data, string subscriptionName)
        {
            this.Event = data;
            this.MessageType = typeof(TEvent).GetTypeInfo();
            this.Metadata = new();
            this.SubscriptionName = subscriptionName;
        }

        public TypeInfo MessageType { get; }

        public Dictionary<string, string> Metadata { get; }

        public string SubscriptionName { get; }

        public TEvent Event { get; }
    }
}
