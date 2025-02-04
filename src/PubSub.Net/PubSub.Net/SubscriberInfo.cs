using PubSub.Net.Interfaces;
using System.Reflection;

namespace PubSub.Net
{
    public class SubscriberInfo
    {
        public SubscriberInfo(TypeInfo subscriberTypeInfo)
        {
            this.SubscriberTypeInfo = subscriberTypeInfo;
            this.HandleEventAsync = new Lazy<MethodInfo>(() => this.SubscriberTypeInfo.GetMethod(nameof(ISubscriber<IEventBase>.HandleEventAsync)));
            this.HandleFailureEventAsync = new Lazy<MethodInfo>(() => this.SubscriberTypeInfo.GetMethod(nameof(ISubscriber<IEventBase>.HandleFailureEventAsync)));
        }

        // the actual type of the subscriber which inherits ISubscriber eg: TestSubscriber<T>
        public TypeInfo SubscriberTypeInfo { get; set; }

        // the type of the TEvent eg: TestEvent
        public TypeInfo MessageType { get; set; }

        // the ISubscrber<TEvent>.HandleEventAsync method which is stored here
        public Lazy<MethodInfo> HandleEventAsync { get; set; }

        // the ISubscrber<TEvent>.HandleFailureEventAsync method which is stored here
        public Lazy<MethodInfo> HandleFailureEventAsync { get; set; }

        // the name of the subscription
        public string SubscriptionName { get; set; }
    }
}
