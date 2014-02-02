using Moq;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Tests.Stubs;

namespace System.Data.Entity.Hooks.Fluent.Tests
{
    [TestFixture]
    internal abstract class InvokeSetupFixture
    {
        [Test]
        public void ShouldInvokeHook_IfAcceptedEntityType()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = new Mock<IDbEntityEntry>();
            dbEntityEntry.Setup(entry => entry.Entity).Returns(new FooEntity());
            dbEntityEntry.Setup(entry => entry.State).Returns(EntityState.Unchanged);

            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);
            setup.Do(s => Assert.Pass("Hook invoked"));

            Assert.That(registeredHook, Is.Not.Null, "Hook not registered");

            registeredHook.HookEntry(dbEntityEntry.Object);

            Assert.Fail("Hook not invoked");
        }

        [Test]
        public void ShouldNotInvokeHook_IfNotAcceptedEntityType()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = new Mock<IDbEntityEntry>();
            dbEntityEntry.Setup(entry => entry.Entity).Returns(new BarEntity());
            dbEntityEntry.Setup(entry => entry.State).Returns(EntityState.Unchanged);

            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);
            setup.Do(s => Assert.Fail("Hook invoked"));

            Assert.That(registeredHook, Is.Not.Null, "Hook not registered");

            registeredHook.HookEntry(dbEntityEntry.Object);

            Assert.Pass("Hook not invoked");
        }

        protected abstract IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar) where T : class;

        protected abstract void SetupRegisterHook(Mock<IDbHookRegistrar> registrar, Action<IDbHook> registerAction);
    }
}
