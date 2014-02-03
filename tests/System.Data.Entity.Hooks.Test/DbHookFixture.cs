using Moq;
using NUnit.Framework;
using System.Data.Entity.Hooks.Test.Stubs;

namespace System.Data.Entity.Hooks.Test
{
    [TestFixture]
    internal sealed class DbHookFixture
    {
        [Test]
        public void ShouldInvokeHookAction_IfAcceptableEntityType()
        {
            var entry = SetupDbEntityEntry(new FooEntityStub(), EntityState.Unchanged);
            var hook = new DbHook<FooEntityStub>(stub => Assert.Pass("Hook invoked"));

            hook.HookEntry(entry);

            Assert.Fail("Hook not invoked");
        }

        [Test]
        [TestCase(EntityState.Added, EntityState.Added)]
        [TestCase(EntityState.Deleted, EntityState.Deleted)]
        [TestCase(EntityState.Detached, EntityState.Detached)]
        [TestCase(EntityState.Modified, EntityState.Modified)]
        [TestCase(EntityState.Unchanged, EntityState.Unchanged)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added, EntityState.Added)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added, EntityState.Deleted)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added, EntityState.Modified)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added, EntityState.Unchanged)]
        public void ShouldInvokeHookAction_IfAcceptableEntityState(EntityState acceptableState, EntityState entityState)
        {
            var entry = SetupDbEntityEntry(new FooEntityStub(), entityState);
            var hook = new DbHook<FooEntityStub>(stub => Assert.Pass("Hook invoked"), acceptableState);

            hook.HookEntry(entry);

            Assert.Fail("Hook not invoked");
        }

        [Test]
        public void ShouldNotInvokeHookAction_IfNotAcceptableEntityType()
        {
            var entry = SetupDbEntityEntry(new object(), EntityState.Unchanged);
            var hook = new DbHook<FooEntityStub>(stub => Assert.Fail("Hook invoked"));

            hook.HookEntry(entry);

            Assert.Pass("Hook not invoked");
        }

        [Test]
        [TestCase(EntityState.Added, EntityState.Deleted)]
        [TestCase(EntityState.Deleted, EntityState.Detached)]
        [TestCase(EntityState.Detached, EntityState.Modified)]
        [TestCase(EntityState.Modified, EntityState.Unchanged)]
        [TestCase(EntityState.Unchanged, EntityState.Added)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Deleted, EntityState.Added)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Added, EntityState.Deleted)]
        [TestCase(EntityState.Unchanged | EntityState.Deleted | EntityState.Added, EntityState.Modified)]
        [TestCase(EntityState.Modified | EntityState.Deleted | EntityState.Added, EntityState.Unchanged)]
        public void ShouldNotInvokeHookAction_IfNotAcceptableEntityState(EntityState acceptableState, EntityState entityState)
        {
            var entry = SetupDbEntityEntry(new FooEntityStub(), entityState);
            var hook = new DbHook<FooEntityStub>(stub => Assert.Fail("Hook invoked"), acceptableState);

            hook.HookEntry(entry);

            Assert.Pass("Hook not invoked");
        }

        private IDbEntityEntry SetupDbEntityEntry(object entity, EntityState entityState)
        {
            var entry = new Mock<IDbEntityEntry>();
            entry.Setup(entityEntry => entityEntry.Entity).Returns(entity);
            entry.Setup(entityEntry => entityEntry.State).Returns(entityState);
            return entry.Object;
        }
    }
}
