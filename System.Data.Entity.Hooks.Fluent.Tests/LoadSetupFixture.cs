using Moq;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;
using System.Data.Entity.Hooks.Fluent.Tests.Stubs;

namespace System.Data.Entity.Hooks.Fluent.Tests
{
    [TestFixture]
    internal class LoadSetupFixture : InvokeSetupFixture
    {
        [Test]
        public void ShouldRegisterLoadHook_OnDo()
        {
            var registrar = new Mock<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);

            setup.Do(s => { });

            registrar.Verify(hookRegistrar => hookRegistrar.RegisterLoadHook(It.IsAny<IDbHook>()), Times.Once);
        }

        [Test]
        public void ShouldNotRegisterSaveHook_OnDo()
        {
            var registrar = new Mock<IDbHookRegistrar>();
            var setup = CreateTypedHookSetup<FooEntity>(registrar.Object);

            setup.Do(s => { });

            registrar.Verify(hookRegistrar => hookRegistrar.RegisterSaveHook(It.IsAny<IDbHook>()), Times.Never);
        }

        protected override IInvokeSetup<T> CreateTypedHookSetup<T>(IDbHookRegistrar dbHookRegistrar)
        {
            return new LoadSetup<T>(dbHookRegistrar);
        }

        protected override void SetupRegisterHook(Mock<IDbHookRegistrar> registrar, Action<IDbHook> registerAction)
        {
            registrar.Setup(hookRegistrar => hookRegistrar.RegisterLoadHook(It.IsAny<IDbHook>())).Callback(registerAction);
        }
    }
}
