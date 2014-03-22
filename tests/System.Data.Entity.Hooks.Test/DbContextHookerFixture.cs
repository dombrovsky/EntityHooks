using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Test.Stubs;

namespace System.Data.Entity.Hooks.Test
{
    internal class DbContextHookerFixture : DbHookRegistrarFixture
    {
        private DbContextHooker _dbContextHooker;

        [SetUp]
        public void Setup()
        {
            DbContextStub.ResetConnections();
        }

        [Test]
        public void ShouldNotInvokeHooks_AfterDispose()
        {
            var dbContext = new DbContextStub();
            var dbContextHooker = new DbContextHooker(dbContext);
            var hook = Substitute.For<IDbHook>();
            dbContextHooker.RegisterSaveHook(hook);

            dbContext.Foos.Add(new FooEntityStub());
            dbContextHooker.Dispose();
            var savedEntities = dbContext.SaveChanges();

            Assert.That(savedEntities, Is.EqualTo(1));
            hook.DidNotReceive().HookEntry(Arg.Any<IDbEntityEntry>());

            dbContext.Dispose();
        }

        protected override void RegisterLoadHook(IDbHook hook)
        {
            _dbContextHooker.RegisterLoadHook(hook);
        }

        protected override void RegisterPreSaveHook(IDbHook hook)
        {
            _dbContextHooker.RegisterSaveHook(hook);
        }

        protected override IDbContext SetupDbContext()
        {
            var dbContext = new DbContextStub();
            _dbContextHooker = CreateContextHooker(dbContext);
            return dbContext;
        }

        protected virtual DbContextHooker CreateContextHooker(DbContext dbContext)
        {
            return new DbContextHooker(dbContext);
        }
    }
}
