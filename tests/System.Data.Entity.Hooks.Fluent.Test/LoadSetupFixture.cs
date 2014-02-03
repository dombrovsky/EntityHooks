using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;
using System.Data.Entity.Hooks.Fluent.Test.Stubs;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal class LoadSetupFixture : InvokeSetupFixture
    {
        [Test]
        public void ShouldRegisterLoadHook_OnDo()
        {
            var registrar = Substitute.For<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar);
            
            setup.Do(s => { });

            registrar.Received(1).RegisterLoadHook(Arg.Any<IDbHook>());
        }

        [Test]
        public void ShouldNotRegisterSaveHook_OnDo()
        {
            var registrar = Substitute.For<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar);

            setup.Do(s => { });

            registrar.DidNotReceive().RegisterSaveHook(Arg.Any<IDbHook>());
        }

        protected override IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar)
        {
            return new LoadSetup<T>(dbHookRegistrar);
        }

        protected override void SetupRegisterHook(IDbHookRegistrar registrar, Action<IDbHook> registerAction)
        {
            registrar.When(hookRegistrar => hookRegistrar.RegisterLoadHook(Arg.Any<IDbHook>())).Do(info => registerAction(info.Arg<IDbHook>()));
        }
    }
}
