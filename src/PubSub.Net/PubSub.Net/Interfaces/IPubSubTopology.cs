namespace PubSub.Net.Interfaces
{
    public interface IPubSubTopology
    {
        IServiceProvider ServiceProvider { get; }

        IReadOnlyList<SubscriberInfo> ReadOnlySubscribers { get; }

        IReadOnlyList<IPipelineMiddleware> ReadOnlyDefaultProcesses { get; }

        bool IsEmulatorActivated { get; }

        IPubSubTopology WithEmulator();

        IPubSubTopology WithMiddleware<T>()
            where T : IPipelineMiddleware;

        IPubSubTopology WithDefaultPublisherPipeline(params IPipelineMiddleware[] defaultPublisherPipelineBuilder);

        IPubSubTopology WithDefaultSubscriberPipeline(params IPipelineMiddleware[] defaultSubscriberPipelineBuilder);

        SubscriberInfo AddSubsciber<TSubscriber, TEvent>(string subscriptionId)
            where TEvent : class, IEventBase;
    }
}
