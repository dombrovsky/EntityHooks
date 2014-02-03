using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal sealed class LoadHookAttacherFixture
    {
        private IDbHookRegistrar _registrar;
        private IDbHook _hook;
        private LoadHookAttacher _hookAttacher;

        [SetUp]
        public void SetUp()
        {
            _registrar = Substitute.For<IDbHookRegistrar>();
            _hook = Substitute.For<IDbHook>();
            _hookAttacher = new LoadHookAttacher(_registrar);
        }

        [Test]
        public void ShouldRegisterLoadHook_OnAttach()
        {
            _hookAttacher.Attach(_hook);

            _registrar.Received(1).RegisterLoadHook(_hook);
        }

        [Test]
        public void ShouldNotRegisterSaveHook_OnAttach()
        {
            _hookAttacher.Attach(_hook);

            _registrar.DidNotReceive().RegisterSaveHook(_hook);
        }

        [Test]
        public void Attach_ShouldReturnSelf()
        {
            Assert.That(_hookAttacher.Attach(_hook), Is.EqualTo(_hookAttacher));
        }
    }
}
