namespace System.Data.Entity.Hooks.Fluent.Internal
{
    /// <summary>
    /// Allows to attach custom <see cref="IDbHook"/> instances on entity save.
    /// </summary>
    internal sealed class SaveHookAttacher : ISaveHookAttacher
    {
        private readonly IDbHookRegistrar _dbHookRegistrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveHookAttacher"/> class.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        public SaveHookAttacher(IDbHookRegistrar dbHookRegistrar)
        {
            _dbHookRegistrar = dbHookRegistrar;
        }

        /// <summary>
        /// Attaches the specified hook to be called for every entity save.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns>
        /// This instance.
        /// </returns>
        public ISaveHookAttacher Attach(IDbHook hook)
        {
            _dbHookRegistrar.RegisterSaveHook(hook);
            return this;
        }
    }
}
