namespace System.Data.Entity.Hooks.Fluent.Internal
{
    /// <summary>
    /// Entry point setup for a hook.
    /// </summary>
    internal sealed class HookSetup : IHookSetup
    {
        private readonly IDbHookRegistrar _dbHookRegistrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="HookSetup"/> class.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        public HookSetup(IDbHookRegistrar dbHookRegistrar)
        {
            _dbHookRegistrar = dbHookRegistrar;
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while saving entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public ISaveSetup<T> OnSave<T>() where T : class
        {
            return new SaveSetup<T>(_dbHookRegistrar);
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while loading entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public ILoadSetup<T> OnLoad<T>() where T : class
        {
            return new LoadSetup<T>(_dbHookRegistrar);
        }
    }
}
