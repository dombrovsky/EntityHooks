using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;
using System.Data.Entity.Hooks.Fluent.Test.Stubs;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal class SaveConditionalSetupFixture : ConditionalSetupFixture
    {
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
        public void ShouldInvokeHook_IfAcceptedEntityState(EntityState acceptableState, EntityState entityState)
        {
            IDbHook registeredHook = null;

            var registrar = Substitute.For<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity(), entityState);
            var setup = CreateConditionalSetup<FooEntity>(registrar, foo => true, acceptableState);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, true);
        }

        [TestCase(EntityState.Added, EntityState.Deleted)]
        [TestCase(EntityState.Deleted, EntityState.Detached)]
        [TestCase(EntityState.Detached, EntityState.Modified)]
        [TestCase(EntityState.Modified, EntityState.Unchanged)]
        [TestCase(EntityState.Unchanged, EntityState.Added)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Deleted, EntityState.Added)]
        [TestCase(EntityState.Unchanged | EntityState.Modified | EntityState.Added, EntityState.Deleted)]
        [TestCase(EntityState.Unchanged | EntityState.Deleted | EntityState.Added, EntityState.Modified)]
        [TestCase(EntityState.Modified | EntityState.Deleted | EntityState.Added, EntityState.Unchanged)]
        public void ShouldNotInvokeHook_IfNotAcceptedEntityState(EntityState acceptableState, EntityState entityState)
        {
            IDbHook registeredHook = null;

            var registrar = Substitute.For<IDbHookRegistrar>();
            SetupRegisterHook(registrar, hook => registeredHook = hook);

            var dbEntityEntry = SetupDbEntityEntry(() => new FooEntity(), entityState);
            var setup = CreateConditionalSetup<FooEntity>(registrar, foo => true, acceptableState);

            ActAndAssert(setup, ref registeredHook, dbEntityEntry, false);
        }

        protected override void SetupRegisterHook(IDbHookRegistrar registrar, Action<IDbHook> registerAction)
        {
            registrar.When(hookRegistrar => hookRegistrar.RegisterSaveHook(Arg.Any<IDbHook>())).Do(info => registerAction(info.Arg<IDbHook>()));
        }

        protected override IConditionalSetup<T> CreateConditionalSetup<T>(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState)
        {
            return new SaveConditionalSetup<T>(dbHookRegistrar, predicate, entityState);
        }
    }
}
