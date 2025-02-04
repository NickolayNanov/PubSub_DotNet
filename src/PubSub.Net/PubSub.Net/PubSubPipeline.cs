using PubSub.Net.Interfaces;

namespace PubSub.Net
{
    public class PubSubPipeline<TEvent>
       where TEvent : class, IEventBase
    {
        private readonly List<Func<PubSubContext<TEvent>, Func<Task>, Task>> middlewares = new();

        public void Use(Func<PubSubContext<TEvent>, Func<Task>, Task> middleware)
        {
            this.middlewares.Add(middleware);
        }

        public async Task ExecuteAsync(PubSubContext<TEvent> context)
        {
            async Task ExecuteMiddleware(int index)
            {
                if (index < this.middlewares.Count)
                {
                    await this.middlewares[index](context, () => ExecuteMiddleware(index + 1));
                }
            }

            await ExecuteMiddleware(0);
        }
    }
}
