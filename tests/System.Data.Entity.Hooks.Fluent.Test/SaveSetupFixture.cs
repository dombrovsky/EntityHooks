using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;
using System.Data.Entity.Hooks.Fluent.Test.Stubs;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal class SaveSetupFixture : InvokeSetupFixture
    {
        [Test]
        public void ShouldRegisterSaveHook_OnDo()
        {
            var registrar = Substitute.For<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar);

            setup.Do(s => { });

            registrar.Received(1).RegisterSaveHook(Arg.Any<IDbHook>());
        }

        [Test]
        public void ShouldNotRegisterLoadHook_OnDo()
        {
            var registrar = Substitute.For<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar);

            setup.Do(s => { });

            registrar.DidNotReceive().RegisterLoadHook(Arg.Any<IDbHook>());
        }

        protected override IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar)
        {
            return new SaveSetup<T>(dbHookRegistrar);
        }

        protected override void SetupRegisterHook(IDbHookRegistrar registrar, Action<IDbHook> registerAction)
        {
            registrar.When(hookRegistrar => hookRegistrar.RegisterSaveHook(Arg.Any<IDbHook>())).Do(info => registerAction(info.Arg<IDbHook>()));
        }
    }
}
