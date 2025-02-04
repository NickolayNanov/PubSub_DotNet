namespace PubSub.Net.Interfaces
{
    public interface ISubscriber<TEvent>
    where TEvent : class, IEventBase
    {
        Task HandleEventAsync(PubSubContext<TEvent> context);

        Task HandleFailureEventAsync(PubSubContext<TEvent> context);
    }
}
