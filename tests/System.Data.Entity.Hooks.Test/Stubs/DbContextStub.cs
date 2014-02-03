namespace System.Data.Entity.Hooks.Test.Stubs
{
    internal class DbContextStub : DbContext, IDbContext
    {
        private static string _connectionId;

        public DbContextStub()
            : base(Effort.DbConnectionFactory.CreatePersistent(_connectionId), true)
        {
        }

        public DbSet<FooEntityStub> Foos { get; set; }

        public static void ResetConnections()
        {
            _connectionId = Guid.NewGuid().ToString();
        }
    }
}
