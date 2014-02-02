using System.Data.Entity.Hooks.Tests.Stubs;
using Moq;
using NUnit.Framework;

namespace System.Data.Entity.Hooks.Tests
{
    internal sealed class DbHookContextFixture : DbHookRegistrarFixture
    {
        private DbHookContextStub _dbHookContext;

        [SetUp]
        public void Setup()
        {
            DbHookContextStub.ResetConnections();
        }

        [Test]
        public void ShouldRunPostSaveHooks_OnSave()
        {
            var dbContext = new DbHookContextStub();
            var hook1 = new Mock<IDbHook>();
            var hook2 = new Mock<IDbHook>();
            dbContext.AddPostSaveHook(hook1.Object);
            dbContext.AddPostSaveHook(hook2.Object);

            var foo = new FooEntityStub();
            dbContext.Foos.Add(foo);
            dbContext.SaveChanges();

            hook1.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Once);
            hook2.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Once);
        }

        protected override void RegisterLoadHook(IDbHook hook)
        {
            _dbHookContext.AddLoadHook(hook);
        }

        protected override void RegisterPreSaveHook(IDbHook hook)
        {
            _dbHookContext.AddPreSaveHook(hook);
        }

        protected override IDbContext SetupDbContext()
        {
            return _dbHookContext = new DbHookContextStub();
        }
    }
}
