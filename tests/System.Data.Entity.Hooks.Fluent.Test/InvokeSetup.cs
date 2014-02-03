using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Test.Stubs;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal abstract class InvokeSetupFixture
    {
        [Test]
        public void ShouldInvokeHook_IfAcceptedEntityType()
        {
            IDbHook registeredHook = null;

            var registrar = Substitute.For<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity(), EntityState.Unchanged);
            var setup = CreateTypedHookSetup<FooEntity>(registrar);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, true);
        }

        [Test]
        public void ShouldNotInvokeHook_IfNotAcceptedEntityType()
        {
            IDbHook registeredHook = null;

            var registrar = Substitute.For<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new BarEntity(), EntityState.Unchanged);
            var setup = CreateTypedHookSetup<FooEntity>(registrar);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, false);
        }

        protected abstract IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar) where T : class;

        protected abstract void SetupRegisterHook(IDbHookRegistrar registrar, Action<IDbHook> registerAction);

        protected IDbEntityEntry SetupDbEntityEntry<T>(Func<T> entityFactory, EntityState entityState) where T : class
        {
            var dbEntityEntry = Substitute.For<IDbEntityEntry>();
            dbEntityEntry.Entity.Returns(info => entityFactory());
            dbEntityEntry.State.Returns(entityState);
            return dbEntityEntry;
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
