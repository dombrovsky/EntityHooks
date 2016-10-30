using System.Data.Entity.Infrastructure;
using System.Threading;
using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Test.Stubs;

namespace System.Data.Entity.Hooks.Test
{
    internal sealed class DbHookContextFixture : DbHookRegistrarFixture
    {
        private DbHookContextStub _dbHookContext;
        private CancellationTokenSource _cancellationTokenSource;

        [SetUp]
        public void Setup()
        {
            DbHookContextStub.ResetConnections();
            _cancellationTokenSource = new CancellationTokenSource();
        }

        [TearDown]
        public void Teardown()
        {
            _cancellationTokenSource.Dispose();
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

#if NET45
        [Test]
        public void ShouldRunPostSaveHooks_OnSaveAsync()
        {
            var dbContext = new DbHookContextStub();
            var hook1 = Substitute.For<IDbHook>();
            var hook2 = Substitute.For<IDbHook>();
            dbContext.AddPostSaveHook(hook1);
            dbContext.AddPostSaveHook(hook2);

            var foo = new FooEntityStub();
            dbContext.Foos.Add(foo);
            dbContext.SaveChangesAsync().Wait();

            hook1.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
            hook2.Received(1).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldRunPostSaveHooks_WhenExceptionOccured_OnSaveAsync()
        {
            var dbContext = new DbHookContextStub();
            var hook1 = Substitute.For<IDbHook>();
            var hook2 = Substitute.For<IDbHook>();
            dbContext.AddPostSaveHook(hook1);
            dbContext.AddPostSaveHook(hook2);

            var sameKey = Guid.NewGuid();
            dbContext.Foos.Add(new FooEntityStub() { Id = sameKey });
            dbContext.Foos.Add(new FooEntityStub() { Id = sameKey });

            try
            {
                dbContext.SaveChangesAsync().Wait(); 
            }
            catch (AggregateException){}

            hook1.Received(2).HookEntry(Arg.Any<IDbEntityEntry>());
            hook2.Received(2).HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void ShouldNotExecutePostSaveHooks_WhenCanceled_OnSaveChangesAsync()
        {
            var dbContext = new DbHookContextStub();
            var hook1 = Substitute.For<IDbHook>();
            dbContext.AddPostSaveHook(hook1);

            var foo = new FooEntityStub();
            dbContext.Foos.Add(foo);
            _cancellationTokenSource.Cancel();

            try
            {
                dbContext.SaveChangesAsync(_cancellationTokenSource.Token).Wait();
            }
            catch (AggregateException) { }

            hook1.DidNotReceive().HookEntry(Arg.Any<IDbEntityEntry>());
        }

        [Test]
        public void PostSaveHookShouldReflectPreSaveAsyncEntityState()
        {
            var dbContext = new DbHookContextStub();
            var hook1 = Substitute.For<IDbHook>();
            dbContext.AddPostSaveHook(hook1);

            var foo = new FooEntityStub();
            dbContext.Foos.Add(foo);
            dbContext.SaveChangesAsync().Wait();

            hook1.Received(1).HookEntry(Arg.Is<IDbEntityEntry>(entry => entry.State == EntityState.Added));
        }
#endif

        [Test]
        public void ShouldRunPostSaveHooks_WhenExceptionOccured_OnSave()
        {
            var dbContext = new DbHookContextStub();
            var hook1 = Substitute.For<IDbHook>();
            var hook2 = Substitute.For<IDbHook>();
            dbContext.AddPostSaveHook(hook1);
            dbContext.AddPostSaveHook(hook2);

            var sameKey = Guid.NewGuid();
            dbContext.Foos.Add(new FooEntityStub() { Id = sameKey });
            dbContext.Foos.Add(new FooEntityStub() { Id = sameKey });

            try
            {
                dbContext.SaveChanges();
            }
            catch (DbUpdateException) { }

            hook1.Received(2).HookEntry(Arg.Any<IDbEntityEntry>());
            hook2.Received(2).HookEntry(Arg.Any<IDbEntityEntry>());
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
