namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Provides possibility to register load and save hooks.
    /// </summary>
    public interface IDbHookRegistrar
    {
        /// <summary>
        /// Registers a hook to run on object materialization stage.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        void RegisterLoadHook(IDbHook dbHook);

        /// <summary>
        /// Registers a hook to run before save data occurs.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        void RegisterSaveHook(IDbHook dbHook);
    }
}
