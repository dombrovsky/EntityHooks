using System.Data.Entity.Hooks.Tests.Stubs;
using System.Data.Entity.Infrastructure;
using Moq;
using NUnit.Framework;

namespace System.Data.Entity.Hooks.Tests
{
    [TestFixture]
    public sealed class DbContextHookerFixture
    {
        private DbContextStub _dbContext;
        private Mock<IDbHook> _hook1;
        private Mock<IDbHook> _hook2;

        [SetUp]
        public void SetUp()
        {
            DbContextStub.ResetConnections();
            _dbContext = new DbContextStub();
            _hook1 = new Mock<IDbHook>();
            _hook2 = new Mock<IDbHook>();
        }

        [TearDown]
        public void TearDown()
        {
            _dbContext.Dispose();
        }

        [Test]
        public void ShouldRunPreSaveHooks_OnSave()
        {
            var dbContextHooker = new DbContextHooker(_dbContext);
            dbContextHooker.RegisterPreSaveHook(_hook1.Object);
            dbContextHooker.RegisterPreSaveHook(_hook2.Object);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.Verify(dbHook => dbHook.HookEntry(_dbContext.Entry(foo)), Times.Once);
            _hook2.Verify(dbHook => dbHook.HookEntry(_dbContext.Entry(foo)), Times.Once);
        }

        [Test]
        public void ShouldRunLoadHooks_OnLoad()
        {
            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            _dbContext = new DbContextStub();
            var dbContextHooker = new DbContextHooker(_dbContext);

            dbContextHooker.RegisterLoadHook(_hook1.Object);
            dbContextHooker.RegisterLoadHook(_hook2.Object);

            _dbContext.Foos.Load();

            _hook1.Verify(dbHook => dbHook.HookEntry(It.IsAny<DbEntityEntry>()), Times.Once);
            _hook2.Verify(dbHook => dbHook.HookEntry(It.IsAny<DbEntityEntry>()), Times.Once);
        }

        [Test]
        public void ShouldNotRunLoadHooks_OnSave()
        {
            var dbContextHooker = new DbContextHooker(_dbContext);
            dbContextHooker.RegisterLoadHook(_hook1.Object);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.Verify(dbHook => dbHook.HookEntry(_dbContext.Entry(foo)), Times.Never);
        }

        [Test]
        public void ShouldNotRunPreSaveHooks_OnLoad()
        {
            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            _dbContext = new DbContextStub();
            var dbContextHooker = new DbContextHooker(_dbContext);
            dbContextHooker.RegisterPreSaveHook(_hook1.Object);

            _dbContext.Foos.Load();

            _hook1.Verify(dbHook => dbHook.HookEntry(_dbContext.Entry(foo)), Times.Never);
        }

        [Test]
        public void ShouldNotRunHooks_AfterDispose()
        {
            var dbContextHooker = new DbContextHooker(_dbContext);
            dbContextHooker.RegisterPreSaveHook(_hook1.Object);

            _dbContext.Foos.Add(new FooEntityStub());
            dbContextHooker.Dispose();
            var savedEntities = _dbContext.SaveChanges();

            Assert.That(savedEntities, Is.EqualTo(1));
            _hook1.Verify(dbHook => dbHook.HookEntry(It.IsAny<DbEntityEntry>()), Times.Never);
        }
    }
}
