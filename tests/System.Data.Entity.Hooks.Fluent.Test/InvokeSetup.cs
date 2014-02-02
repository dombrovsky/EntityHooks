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

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity(), EntityState.Unchanged);
            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, true);
        }

        [Test]
        public void ShouldNotInvokeHook_IfNotAcceptedEntityType()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new BarEntity(), EntityState.Unchanged);
            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, false);
        }

        protected abstract IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar) where T : class;

        protected abstract void SetupRegisterHook(Mock<IDbHookRegistrar> registrar, Action<IDbHook> registerAction);

        protected IDbEntityEntry SetupDbEntityEntry<T>(Func<T> entityFactory, EntityState entityState) where T : class
        {
            var dbEntityEntry = new Mock<IDbEntityEntry>();
            dbEntityEntry.Setup(entry => entry.Entity).Returns(entityFactory);
            dbEntityEntry.Setup(entry => entry.State).Returns(entityState);
            return dbEntityEntry.Object;
        }

        protected void ActAndAssert<T>(IInvokeSetup<T> setup, ref IDbHook registeredHook, IDbEntityEntry dbEntityEntry, bool shouldInvokeHook) where T : class
        {
            setup.Do(
                s =>
                {
                    if (shouldInvokeHook)
                    {
                        Assert.Pass("Hook invoked");
                    }
                    else
                    {
                        Assert.Fail("Hook invoked");
                    }
                });

            Assert.That(registeredHook, Is.Not.Null, "Hook not registered");

            registeredHook.HookEntry(dbEntityEntry);

            if (!shouldInvokeHook)
            {
                Assert.Pass("Hook not invoked");
            }
            else
            {
                Assert.Fail("Hook not invoked");
            }
        }
    }
}
