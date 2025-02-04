using PubSub.Net.Interfaces;

namespace Test.PubSub.Net.WebApi
{
    public class DbTransactionService : IDbTransactionService
    {
        private readonly ApplicationDbContext dbContext;

        public DbTransactionService(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task RollBackTransactionAsync()
        {
            await this.dbContext.Database.RollbackTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await this.dbContext.Database.CommitTransactionAsync();
        }

        public async Task BeginTransactionAsync()
        {
            await this.dbContext.Database.BeginTransactionAsync();
        }
    }
}
