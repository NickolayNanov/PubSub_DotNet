using PubSub.Net.Enums;

namespace PubSub.Net.Interfaces
{
    public interface IPipelineMiddleware
    {
        PrePost PrePost { get; }

        PipelineType PipelineType { get; }

        Task InvokeAsync<TEvent>(PubSubContext<TEvent> context, Func<Task> next)
            where TEvent : class, IEventBase;
    }
}
