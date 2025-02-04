using PubSub.Net;
using PubSub.Net.Db;
using PubSub.Net.Interfaces;

namespace Test.PubSub.Net.WebApi.Pubub
{
    public record TestEvent : IEventBase
    {
        public Guid Id { get; set; }
        public DateTime PublishedOn { get; set; }
        public string PublishedBy { get; set; }
        public string TransactionIdentifier { get; set; }
    }

    public class TestSubscriber : DbTransactionSubscriber<TestEvent>
    {
        public TestSubscriber(IDbTransactionService dbTransactionService)
            : base(dbTransactionService)
        {
        }

        protected override Task InternalHandleEventAsync(PubSubContext<TestEvent> context)
        {
            throw new NotImplementedException();
        }

        protected override Task InternalHandleFailureEventAsync(PubSubContext<TestEvent> context)
        {
            throw new NotImplementedException();
        }
    }
}
