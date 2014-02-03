namespace System.Data.Entity.Hooks.Test.Stubs
{
    internal interface IDbContext : IDisposable
    {
        DbSet<FooEntityStub> Foos { get; }

        int SaveChanges();
    }
}
