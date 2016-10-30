using System.Data.Entity.Infrastructure;
using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Test.Stubs;

namespace System.Data.Entity.Hooks.Test
{
    [TestFixture]
    internal abstract class DbHookRegistrarFixture
    {
        private IDbHook _hook1;
        private IDbHook _hook2;
        private IDbContext _dbContext;

        [SetUp]
        public void SetUp()
        {
            _hook1 = Substitute.For<IDbHook>();
            _hook2 = Substitute.For<IDbHook>();
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
            RegisterPreSaveHook(_hook1);
            RegisterPreSaveHook(_hook2);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldRunPreSaveHooks_OnSaveAsync()
        {
            _dbContext = SetupDbContext();
            RegisterPreSaveHook(_hook1);
            RegisterPreSaveHook(_hook2);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChangesAsync().Wait();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
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

            RegisterLoadHook(_hook1);
            RegisterLoadHook(_hook2);

            _dbContext.Foos.Load();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldRunLoadHooks_OnLoad_ForUntrackedEntities()
        {
            var foo = new FooEntityStub();
            _dbContext = SetupDbContext();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            _dbContext = SetupDbContext();

            RegisterLoadHook(_hook1);

            _dbContext.Foos.AsNoTracking().Load();

            _hook1.Received(1).HookEntry(Arg.Is<IDbEntityEntry>(entry => entry.State == EntityState.Detached));
        }

        [Test]
        public void ShouldRunPreSaveHooks_OnSave_ForAttachedEntities()
        {
            _dbContext = SetupDbContext();
            RegisterPreSaveHook(_hook1);
            RegisterPreSaveHook(_hook2);

            var foo = new FooEntityStub();
            _dbContext.Foos.Attach(foo);
            _dbContext.SaveChanges();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldRunPreSaveHooks_OnSaveAsync_ForAttachedEntities()
        {
            _dbContext = SetupDbContext();
            RegisterPreSaveHook(_hook1);
            RegisterPreSaveHook(_hook2);

            var foo = new FooEntityStub();
            _dbContext.Foos.Attach(foo);
            _dbContext.SaveChangesAsync().Wait();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldNotRunLoadHooks_OnSave()
        {
            _dbContext = SetupDbContext();
            RegisterLoadHook(_hook1);

            var foo = new FooEntityStub();
            _dbContext.Foos.Add(foo);
            _dbContext.SaveChanges();

            _hook1.DidNotReceive().HookEntry(Arg.Any<IDbEntityEntry>());
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
            RegisterPreSaveHook(_hook1);

            _dbContext.Foos.Load();

            _hook1.DidNotReceive().HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void IfEntityStateChangedBySaveHook_NextHookShouldBeCalledWithNewState()
        {
            _dbContext = SetupDbContext();
            _hook1
                .When(hook => hook.HookEntry(Arg.Any<IDbEntityEntry>()))
                .Do(info =>
                    {
                        var foo = (FooEntityStub) info.Arg<IDbEntityEntry>().Entity;
                        _dbContext.Foos.Remove(foo);
                    });

            RegisterPreSaveHook(_hook1);
            RegisterPreSaveHook(_hook2);

            _dbContext.Foos.Add(new FooEntityStub());
            _dbContext.SaveChanges();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Is<IDbEntityEntry>(entry => entry.State == EntityState.Detached));
        }

        [Test]
        public void IfEntityStateChangedByLoadHook_NextHookShouldBeCalledWithNewState()
        {
            _dbContext = SetupDbContext();
            _dbContext.Foos.Add(new FooEntityStub());
            _dbContext.SaveChanges();
            _dbContext.Dispose();
            _dbContext = SetupDbContext();

            _hook1
                .When(hook => hook.HookEntry(Arg.Any<IDbEntityEntry>()))
                .Do(info =>
                {
                    var foo = (FooEntityStub)info.Arg<IDbEntityEntry>().Entity;
                    _dbContext.Foos.Remove(foo);
                });

            RegisterLoadHook(_hook1);
            RegisterLoadHook(_hook2);

            _dbContext.Foos.Load();

            _hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            _hook2.Received(1).HookEntry(Arg.Is<IDbEntityEntry>(entry => entry.State == EntityState.Deleted));
        }
        
        [Test]
        public void ShouldRethrowOriginalException_OnSaveChangesAsync()
        {
            var dbContext = SetupDbContext();

            var sameKey = Guid.NewGuid();
            dbContext.Foos.Add(new FooEntityStub() { Id = sameKey });
            dbContext.Foos.Add(new FooEntityStub() { Id = sameKey });

            Assert.Throws<DbUpdateException>(async () => { await dbContext.SaveChangesAsync(); });
        }

        protected abstract void RegisterLoadHook(IDbHook hook);

        protected abstract void RegisterPreSaveHook(IDbHook hook);

        protected abstract IDbContext SetupDbContext();
    }
}
