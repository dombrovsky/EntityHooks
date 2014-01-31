using System.Data.Entity.Hooks.Tests.Stubs;
using Moq;
using NUnit.Framework;

namespace System.Data.Entity.Hooks.Tests
{
    internal sealed class DbContextHookerFixture : HookerFixture
    {
        private DbContextHooker _dbContextHooker;

        [SetUp]
        public void Setup()
        {
            DbContextStub.ResetConnections();
        }

        [Test]
        public void ShouldNotRunHooks_AfterDispose()
        {
            var dbContext = new DbContextStub();
            var dbContextHooker = new DbContextHooker(dbContext);
            var hook = new Mock<IDbHook>();
            dbContextHooker.RegisterPreSaveHook(hook.Object);

            dbContext.Foos.Add(new FooEntityStub());
            dbContextHooker.Dispose();
            var savedEntities = dbContext.SaveChanges();

            Assert.That(savedEntities, Is.EqualTo(1));
            hook.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Never);

            dbContext.Dispose();
        }

        protected override void RegisterLoadHook(IDbHook hook)
        {
            _dbContextHooker.RegisterLoadHook(hook);
        }

        protected override void RegisterPreSaveHook(IDbHook hook)
        {
            _dbContextHooker.RegisterPreSaveHook(hook);
        }

        protected override IDbContext SetupDbContext()
        {
            var dbContext = new DbContextStub();
            _dbContextHooker = new DbContextHooker(dbContext);
            return dbContext;
        }
    }
}
