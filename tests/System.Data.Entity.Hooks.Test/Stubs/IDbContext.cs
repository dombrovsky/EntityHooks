#if NET45
using System.Threading;
using System.Threading.Tasks;
#endif

namespace System.Data.Entity.Hooks.Test.Stubs
{
    internal interface IDbContext : IDisposable
    {
        DbSet<FooEntityStub> Foos { get; }

        int SaveChanges();

#if NET45
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);

        Task<int> SaveChangesAsync();
#endif
    }
}
