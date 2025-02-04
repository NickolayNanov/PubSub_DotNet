namespace PubSub.Net.Interfaces
{
    public interface IEventBase
    {
        public Guid Id { get; set; }

        public DateTime PublishedOn { get; set; }

        public string PublishedBy { get; set; }

        public string TransactionIdentifier { get; set; }
    }
}
