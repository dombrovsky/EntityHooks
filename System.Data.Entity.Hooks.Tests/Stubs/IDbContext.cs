namespace System.Data.Entity.Hooks.Tests.Stubs
{
    internal interface IDbContext : IDisposable
    {
        DbSet<FooEntityStub> Foos { get; }

        int SaveChanges();
    }
}
