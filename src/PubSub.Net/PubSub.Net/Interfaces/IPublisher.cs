namespace PubSub.Net.Interfaces
{
    public interface IPublisher
    {
        public Task PublishMessageAsync<TEvent>(string topicId, TEvent @event)
            where TEvent : IEventBase;
    }
}
