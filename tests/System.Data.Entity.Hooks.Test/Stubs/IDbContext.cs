using System.Threading;
using System.Threading.Tasks;

namespace System.Data.Entity.Hooks.Test.Stubs
{
    internal interface IDbContext : IDisposable
    {
        DbSet<FooEntityStub> Foos { get; }

        int SaveChanges();

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsync();
    }
}
