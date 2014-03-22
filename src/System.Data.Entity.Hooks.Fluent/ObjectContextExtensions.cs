using System.Data.Entity.Core.Objects;
using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// <see cref="ObjectContext"/> extensions.
    /// </summary>
    public static class ObjectContextExtensions
    {
        /// <summary>
        /// Returns <see cref="IDbHookRegistrar" /> for the <see cref="ObjectContext" />.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <returns>
        /// Hook registrar.
        /// </returns>
        public static IDbHookRegistrar AsHookable(this ObjectContext objectContext)
        {
            return new DbContextHooker(objectContext);
        }

        /// <summary>
        /// Returns Fluent API interface for attaching hooks on <see cref="ObjectContext" />.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <returns>
        /// Fluent api entry point.
        /// </returns>
        public static IHookSetup CreateHook(this ObjectContext objectContext)
        {
            return new HookSetup(objectContext.AsHookable());
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while saving entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="objectContext">The object context.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public static ISaveSetup<T> OnSave<T>(this ObjectContext objectContext) where T : class
        {
            return objectContext.CreateHook().OnSave<T>();
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while loading entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="objectContext">The object context.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public static ILoadSetup<T> OnLoad<T>(this ObjectContext objectContext) where T : class
        {
            return objectContext.CreateHook().OnLoad<T>();
        }

        /// <summary>
        /// Allows to attach hooks that should be called while saving entities.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <returns>
        /// Hook attacher.
        /// </returns>
        public static ISaveHookAttacher OnSave(this ObjectContext objectContext)
        {
            return new SaveHookAttacher(objectContext.AsHookable());
        }

        /// <summary>
        /// Allows to attach hooks that should be called while loading entities.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        /// <returns>
        /// Hook attacher.
        /// </returns>
        public static ILoadHookAttacher OnLoad(this ObjectContext objectContext)
        {
            return new LoadHookAttacher(objectContext.AsHookable());
        }
    }
}
