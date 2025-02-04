namespace PubSub.Net.Interfaces
{
    public interface IDbTransactionService
    {
        Task RollBackTransactionAsync();

        Task CommitTransactionAsync();

        Task BeginTransactionAsync();
    }
}
