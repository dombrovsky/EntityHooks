namespace System.Data.Entity.Hooks.Fluent.Internal
{
    /// <summary>
    /// Allows to attach custom <see cref="IDbHook"/> instances on entity load.
    /// </summary>
    internal sealed class LoadHookAttacher : ILoadHookAttacher
    {
        private readonly IDbHookRegistrar _dbHookRegistrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadHookAttacher"/> class.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        public LoadHookAttacher(IDbHookRegistrar dbHookRegistrar)
        {
            _dbHookRegistrar = dbHookRegistrar;
        }

        /// <summary>
        /// Attaches the specified hook to be called for every entity load.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns>
        /// This instance.
        /// </returns>
        public ILoadHookAttacher Attach(IDbHook hook)
        {
            _dbHookRegistrar.RegisterLoadHook(hook);
            return this;
        }
    }
}
