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

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity { Foo = 42 }, EntityState.Unchanged);

            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, true);
        }

        [Test]
        public void ShouldNotInvokeHook_IfEntityNotSatisfiesCondition()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity {Foo = 0}, EntityState.Unchanged);
            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, false);
        }

        [Test]
        public void ShouldInvokeHook_IfEntitySatisfiesAllConditions()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity { Foo = 42, Bar = 11}, EntityState.Unchanged);

            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
            setup = setup.And(foo => foo.Bar == 11);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, true);
        }

        [Test]
        public void ShouldNotInvokeHook_IfEntitySatisfiesNotAllConditions()
        {
            IDbHook registeredHook = null;

            var registrar = new Mock<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity { Foo = 42, Bar = 11 }, EntityState.Unchanged);

            var setup = CreateConditionalSetup<FooEntity>(registrar.Object, foo => foo.Foo == 42, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
            setup = setup.And(foo => foo.Bar == 42);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, false);
        }

        protected abstract IConditionalSetup<T> CreateConditionalSetup<T>(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState) where T : class;

        protected override IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar)
        {
            return CreateConditionalSetup<T>(dbHookRegistrar, obj => true, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
        }
    }
}
