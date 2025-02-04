using PubSub.Net.Enums;
using PubSub.Net.Interfaces;
using PubSub.Net;

namespace Test.PubSub.Net.WebApi.Pubub
{
    public class PrePubTestProcess : IPipelineMiddleware
    {
        public PipelineType PipelineType => PipelineType.SubPipeline;

        public PrePost PrePost => PrePost.Pre;

        public async Task InvokeAsync<TEvent>(PubSubContext<TEvent> context, Func<Task> next)
            where TEvent : class, IEventBase
        {
            context.Metadata.Add("test", "test");
            await next();
        }
    }
}
