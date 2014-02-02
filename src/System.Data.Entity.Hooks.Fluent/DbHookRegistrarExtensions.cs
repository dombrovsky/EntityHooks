using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// <see cref="IDbHookRegistrar"/> extensions.
    /// </summary>
    public static class DbHookRegistrarExtensions
    {
        /// <summary>
        /// Returns Fluent API interface for attaching hooks on <see cref="IDbHookRegistrar"/>.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <returns>Fluent api entry point.</returns>
        public static IHookSetup CreateHook(this IDbHookRegistrar dbHookRegistrar)
        {
            return new HookSetup(dbHookRegistrar);
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while saving entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <returns>The setup for a hook.</returns>
        public static ISaveSetup<T> OnSave<T>(this IDbHookRegistrar dbHookRegistrar) where T : class 
        {
            return dbHookRegistrar.CreateHook().OnSave<T>();
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while loading entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <returns>The setup for a hook.</returns>
        public static ILoadSetup<T> OnLoad<T>(this IDbHookRegistrar dbHookRegistrar) where T : class
        {
            return dbHookRegistrar.CreateHook().OnLoad<T>();
        }
    }
}
