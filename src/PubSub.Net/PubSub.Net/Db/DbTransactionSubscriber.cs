using PubSub.Net.Interfaces;

namespace PubSub.Net.Db
{
    public abstract class DbTransactionSubscriber<TEvent> : ISubscriber<TEvent>
        where TEvent : class, IEventBase
    {
        private readonly IDbTransactionService dbTransactionService;

        protected DbTransactionSubscriber(
            IDbTransactionService dbTransactionService)
        {
            this.dbTransactionService = dbTransactionService;
        }

        public async Task HandleEventAsync(PubSubContext<TEvent> context)
        {
            try
            {
                await this.dbTransactionService.BeginTransactionAsync();
                await this.InternalHandleEventAsync(context);
                await this.dbTransactionService.CommitTransactionAsync();
            }
            catch
            {
                await this.HandleFailureEventAsync(context);
            }
        }

        public async Task HandleFailureEventAsync(PubSubContext<TEvent> context)
        {
            await this.InternalHandleFailureEventAsync(context);
            await this.dbTransactionService.RollBackTransactionAsync();
        }

        protected abstract Task InternalHandleEventAsync(PubSubContext<TEvent> context);

        protected abstract Task InternalHandleFailureEventAsync(PubSubContext<TEvent> context);
    }
}
