using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.DataBase
{
    public class SimpleDbContextFactory<TContext> : IDbContextFactory<TContext>
        where TContext : DbContext
    {
        private readonly DbContextOptions<TContext> _options;

        public SimpleDbContextFactory(DbContextOptions<TContext> options)
        {
            _options = options;
        }

        public TContext CreateDbContext()
        {
            return (TContext)Activator.CreateInstance(typeof(TContext), _options)!;
        }

        public Task<TContext> CreateDbContextAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult(CreateDbContext());
        }
    }
}
