using System.Data.Entity.Hooks.Fluent.Internal;

namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// <see cref="DbContext"/> extensions.
    /// </summary>
    public static class DbContextExtensions
    {
        /// <summary>
        /// Returns <see cref="IDbHookRegistrar"/> for the <see cref="DbContext"/>.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>Hook registrar.</returns>
        public static IDbHookRegistrar AsHookable(this DbContext dbContext)
        {
            return dbContext as IDbHookRegistrar ?? new DbContextHooker(dbContext);
        }

        /// <summary>
        /// Returns Fluent API interface for attaching hooks on <see cref="DbContext" />.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>
        /// Fluent api entry point.
        /// </returns>
        public static IHookSetup CreateHook(this DbContext dbContext)
        {
            return new HookSetup(dbContext.AsHookable());
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while saving entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public static ISaveSetup<T> OnSave<T>(this DbContext dbContext) where T : class
        {
            return dbContext.CreateHook().OnSave<T>();
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while loading entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <param name="dbContext">The database context.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public static ILoadSetup<T> OnLoad<T>(this DbContext dbContext) where T : class
        {
            return dbContext.CreateHook().OnLoad<T>();
        }

        /// <summary>
        /// Allows to attach hooks that should be called while saving entities.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>Hook attacher.</returns>
        public static ISaveHookAttacher OnSave(this DbContext dbContext)
        {
            return new SaveHookAttacher(dbContext.AsHookable());
        }

        /// <summary>
        /// Allows to attach hooks that should be called while loading entities.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        /// <returns>Hook attacher.</returns>
        public static ILoadHookAttacher OnLoad(this DbContext dbContext)
        {
            return new LoadHookAttacher(dbContext.AsHookable());
        }
    }
}
