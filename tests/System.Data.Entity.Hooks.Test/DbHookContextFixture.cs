using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Test.Stubs;

namespace System.Data.Entity.Hooks.Test
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
            var hook1 = Substitute.For<IDbHook>();
            var hook2 = Substitute.For<IDbHook>();
            dbContext.AddPostSaveHook(hook1);
            dbContext.AddPostSaveHook(hook2);

            var foo = new FooEntityStub();
            dbContext.Foos.Add(foo);
            dbContext.SaveChanges();

            hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void PostSaveHookShouldReflectPreSaveEntityState()
        {
            var dbContext = new DbHookContextStub();
            var hook1 = Substitute.For<IDbHook>();
            dbContext.AddPostSaveHook(hook1);

            var foo = new FooEntityStub();
            dbContext.Foos.Add(foo);
            dbContext.SaveChanges();

            hook1.Received(1).HookEntry(Arg.Is<IDbEntityEntry>(entry => entry.State == EntityState.Added));
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
