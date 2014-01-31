namespace System.Data.Entity.Hooks.Tests.Stubs
{
    internal class DbContextStub : DbContext
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
