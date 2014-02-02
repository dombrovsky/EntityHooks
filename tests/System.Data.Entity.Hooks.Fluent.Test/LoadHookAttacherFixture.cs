using Moq;
using NUnit.Framework;
using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent.Test
{
    [TestFixture]
    internal sealed class LoadHookAttacherFixture
    {
        private Mock<IDbHookRegistrar> _registrar;
        private Mock<IDbHook> _hook;
        private LoadHookAttacher _hookAttacher;

        [SetUp]
        public void SetUp()
        {
            _registrar = new Mock<IDbHookRegistrar>();
            _hook = new Mock<IDbHook>();
            _hookAttacher = new LoadHookAttacher(_registrar.Object);
        }

        [Test]
        public void ShouldRegisterLoadHook_OnAttach()
        {
            _hookAttacher.Attach(_hook.Object);

            _registrar.Verify(hookRegistrar => hookRegistrar.RegisterLoadHook(_hook.Object), Times.Once);
        }

        [Test]
        public void ShouldNotRegisterSaveHook_OnAttach()
        {
            _hookAttacher.Attach(_hook.Object);

            _registrar.Verify(hookRegistrar => hookRegistrar.RegisterSaveHook(_hook.Object), Times.Never);
        }

        [Test]
        public void Attach_ShouldReturnSelf()
        {
            Assert.That(_hookAttacher.Attach(_hook.Object), Is.EqualTo(_hookAttacher));
        }
    }
}
