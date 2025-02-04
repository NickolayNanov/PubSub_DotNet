using Microsoft.Extensions.DependencyInjection;
using PubSub.Net.Interfaces;
using System.Reflection;

namespace PubSub.Net
{
    public class PubSubTopology : IPubSubTopology
    {
        private readonly IServiceProvider serviceProvider;

        public PubSubTopology(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IReadOnlyList<SubscriberInfo> ReadOnlySubscribers => this.Subscribers.AsReadOnly();

        public IReadOnlyList<IPipelineMiddleware> ReadOnlyDefaultProcesses => this.DefaultProcess.AsReadOnly();

        public IServiceProvider ServiceProvider => this.serviceProvider;

        public bool IsEmulatorActivated { get; private set; }

        private List<SubscriberInfo> Subscribers { get; } = new();

        private List<IPipelineMiddleware> DefaultProcess { get; } = new();

        public IPubSubTopology WithEmulator()
        {
            this.IsEmulatorActivated = true;
            return this;
        }

        public IPubSubTopology WithDefaultPublisherPipeline(params IPipelineMiddleware[] defaultPublisherPipelineBuilder)
        {
            this.DefaultProcess.AddRange(defaultPublisherPipelineBuilder);
            return this;
        }

        public IPubSubTopology WithDefaultSubscriberPipeline(params IPipelineMiddleware[] defaultSubscriberPipelineBuilder)
        {
            this.DefaultProcess.AddRange(defaultSubscriberPipelineBuilder);
            return this;
        }

        public SubscriberInfo AddSubsciber<TSubscriber, TEvent>(string subscriptionId)
            where TEvent : class, IEventBase
        {
            var subscriberInfo = new SubscriberInfo(typeof(TSubscriber).GetTypeInfo())
            {
                SubscriptionName = subscriptionId,
                MessageType = typeof(TEvent).GetTypeInfo(),
            };

            this.Subscribers.Add(subscriberInfo);

            return subscriberInfo;
        }

        public IPubSubTopology WithMiddleware<T>()
            where T : IPipelineMiddleware
        {
            var type = typeof(T);
            var middleware = (T)this.serviceProvider.GetRequiredService(type);
            this.DefaultProcess.Add(middleware);

            return this;
        }
    }
}
