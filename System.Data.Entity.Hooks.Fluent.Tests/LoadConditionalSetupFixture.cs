using Moq;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent.Tests
{
    [TestFixture]
    internal class LoadConditionalSetupFixture : ConditionalSetupFixture
    {
        protected override void SetupRegisterHook(Mock<IDbHookRegistrar> registrar, Action<IDbHook> registerAction)
        {
            registrar.Setup(hookRegistrar => hookRegistrar.RegisterLoadHook(It.IsAny<IDbHook>())).Callback(registerAction);
        }

        protected override IConditionalSetup<T> CreateConditionalSetup<T>(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState)
        {
            return new LoadConditionalSetup<T>(dbHookRegistrar, predicate);
        }
    }
}
