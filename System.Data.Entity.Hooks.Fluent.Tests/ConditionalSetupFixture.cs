using Moq;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Tests.Stubs;

namespace System.Data.Entity.Hooks.Fluent.Tests
{
    [TestFixture]
    internal abstract class ConditionalSetupFixture : InvokeSetupFixture
    {
        [Test]
        public void ShouldInvokeHook_IfEntitySatisfiesCondition()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = new Mock<IDbEntityEntry>();
            dbEntityEntry.Setup(entry => entry.Entity).Returns(new FooEntity {Foo = 42});
            dbEntityEntry.Setup(entry => entry.State).Returns(EntityState.Unchanged);

            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);

            setup.Do(s => Assert.Pass("Hook invoked"));

            Assert.That(registeredHook, Is.Not.Null, "Hook not registered");

            registeredHook.HookEntry(dbEntityEntry.Object);

            Assert.Fail("Hook not invoked");
        }

        [Test]
        public void ShouldNotInvokeHook_IfEntityNotSatisfiesCondition()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = new Mock<IDbEntityEntry>();
            dbEntityEntry.Setup(entry => entry.Entity).Returns(new FooEntity { Foo = 0 });
            dbEntityEntry.Setup(entry => entry.State).Returns(EntityState.Unchanged);

            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
            setup.Do(s => Assert.Fail("Hook invoked"));

            Assert.That(registeredHook, Is.Not.Null, "Hook not registered");

            registeredHook.HookEntry(dbEntityEntry.Object);

            Assert.Pass("Hook not invoked");
        }

        [Test]
        public void ShouldInvokeHook_IfEntitySatisfiesAllConditions()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = new Mock<IDbEntityEntry>();
            dbEntityEntry.Setup(entry => entry.Entity).Returns(new FooEntity { Foo = 42, Bar = 11 });
            dbEntityEntry.Setup(entry => entry.State).Returns(EntityState.Unchanged);

            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
            setup = setup.And(foo => foo.Bar == 11);

            setup.Do(s => Assert.Pass("Hook invoked"));

            Assert.That(registeredHook, Is.Not.Null, "Hook not registered");

            registeredHook.HookEntry(dbEntityEntry.Object);

            Assert.Fail("Hook not invoked");
        }

        [Test]
        public void ShouldNotInvokeHook_IfEntitySatisfiesNotAllConditions()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = new Mock<IDbEntityEntry>();
            dbEntityEntry.Setup(entry => entry.Entity).Returns(new FooEntity { Foo = 42, Bar = 11 });
            dbEntityEntry.Setup(entry => entry.State).Returns(EntityState.Unchanged);

            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
            setup = setup.And(foo => foo.Bar == 42);

            setup.Do(s => Assert.Fail("Hook invoked"));

            Assert.That(registeredHook, Is.Not.Null, "Hook not registered");

            registeredHook.HookEntry(dbEntityEntry.Object);

            Assert.Pass("Hook not invoked");
        }

        protected abstract IConditionalSetup<T> CreateConditionalSetup<T>(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState) where T : class;

        protected override IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar)
        {
            return CreateConditionalSetup<T>(dbHookRegistrar, obj => true, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
        }
    }
}
