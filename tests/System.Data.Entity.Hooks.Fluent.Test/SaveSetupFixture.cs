using Moq;
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
            var registrar = new Mock<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);

            setup.Do(s => { });

            registrar.Verify(hookRegistrar => hookRegistrar.RegisterSaveHook(It.IsAny<IDbHook>()), Times.Once);
        }

        [Test]
        public void ShouldNotRegisterLoadHook_OnDo()
        {
            var registrar = new Mock<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);

            setup.Do(s => { });

            registrar.Verify(hookRegistrar => hookRegistrar.RegisterLoadHook(It.IsAny<IDbHook>()), Times.Never);
        }

        protected override IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar)
        {
            return new SaveSetup<T>(dbHookRegistrar);
        }

        protected override void SetupRegisterHook(Mock<IDbHookRegistrar> registrar, Action<IDbHook> registerAction)
        {
            registrar.Setup(hookRegistrar => hookRegistrar.RegisterSaveHook(It.IsAny<IDbHook>())).Callback(registerAction);
        }
    }
}
