using Moq;
using NUnit.Framework;
using System.Data.Entity.Hooks.Tests.Stubs;

namespace System.Data.Entity.Hooks.Tests
{
    [TestFixture]
    internal abstract class DbHookRegistrarFixture
    {
        private Mock<IDbHook> _hook1;
        private Mock<IDbHook> _hook2;
        private IDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _hook1 = new Mock<IDbHook>();
            _hook2 = new Mock<IDbHook>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();   
            }
        }

        [Test]
        public void ShouldRunPreSaveHooks_OnSave()
        {
            _dbContext = SetupDbContext();
            RegisterPreSaveHook(_hook1.Object);
            RegisterPreSaveHook(_hook2.Object);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Once);
            _hook2.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Once);
        }

        [Test]
        public void ShouldRunLoadHooks_OnLoad()
        {
            var foo = new FooEntityStub();
            _dbContext = SetupDbContext();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            _dbContext = SetupDbContext();

            RegisterLoadHook(_hook1.Object);
            RegisterLoadHook(_hook2.Object);

            _dbContext.Foos.Load();

            _hook1.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Once);
            _hook2.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Once);
        }

        [Test]
        public void ShouldNotRunLoadHooks_OnSave()
        {
            _dbContext = SetupDbContext();
            RegisterLoadHook(_hook1.Object);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Never);
        }

        [Test]
        public void ShouldNotRunPreSaveHooks_OnLoad()
        {
            var foo = new FooEntityStub();
            _dbContext = SetupDbContext();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            _dbContext = SetupDbContext();
            RegisterPreSaveHook(_hook1.Object);

            _dbContext.Foos.Load();

            _hook1.Verify(dbHook => dbHook.HookEntry(It.IsAny<IDbEntityEntry>()), Times.Never);
        }

        protected abstract void RegisterLoadHook(IDbHook hook);

        protected abstract void RegisterPreSaveHook(IDbHook hook);

        protected abstract IDbContext SetupDbContext();
    }
}
