using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal class LoadConditionalSetupFixture : ConditionalSetupFixture
    {
        protected override void SetupRegisterHook(IDbHookRegistrar registrar, Action<IDbHook> registerAction)
        {
            registrar.When(hookRegistrar => hookRegistrar.RegisterLoadHook(Arg.Any<IDbHook>())).Do(info => registerAction(info.Arg<IDbHook>()));
        }

        protected override IConditionalSetup<T> CreateConditionalSetup<T>(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState)
        {
            return new LoadConditionalSetup<T>(dbHookRegistrar, predicate);
        }
    }
}
