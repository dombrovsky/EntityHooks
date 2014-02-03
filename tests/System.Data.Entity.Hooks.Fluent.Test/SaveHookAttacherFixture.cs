using NSubstitute;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal sealed class SaveHookAttacherFixture
    {
        private IDbHookRegistrar _registrar;
        private IDbHook _hook;
        private SaveHookAttacher _hookAttacher;

        [SetUp]
        public void SetUp()
        {
            _registrar = Substitute.For<IDbHookRegistrar>();
            _hook = Substitute.For<IDbHook>();
            _hookAttacher = new SaveHookAttacher(_registrar);
        }
        
        [Test]
        public void ShouldRegisterSaveHook_OnAttach()
        {
            _hookAttacher.Attach(_hook);

            _registrar.Received(1).RegisterSaveHook(_hook);
        }

        [Test]
        public void ShouldNotRegisterLoadHook_OnAttach()
        {
            _hookAttacher.Attach(_hook);

            _registrar.DidNotReceive().RegisterLoadHook(_hook);
        }

        [Test]
        public void Attach_ShouldReturnSelf()
        {
            Assert.That(_hookAttacher.Attach(_hook), Is.EqualTo(_hookAttacher));
        }
    }
}
