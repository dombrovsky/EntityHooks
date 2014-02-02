namespace System.Data.Entity.Hooks.Fluent.Internal
{
    /// <summary>
    /// Setup for a hook, that should be called while loading entity of specified type.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    internal sealed class LoadSetup<T> : ILoadSetup<T> where T : class
    {
        private readonly IDbHookRegistrar _dbHookRegistrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="LoadSetup{T}"/> class.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        public LoadSetup(IDbHookRegistrar dbHookRegistrar)
        {
            _dbHookRegistrar = dbHookRegistrar;
        }

        /// <summary>
        /// Sets the action to be invoked by hook.
        /// </summary>
        /// <param name="hookAction">The hook action.</param>
        public void Do(Action<T> hookAction)
        {
            _dbHookRegistrar.RegisterLoadHook(new DbHook<T>(hookAction));
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called when entity satisfies specified condition.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public IConditionalSetup<T> When(Predicate<T> predicate)
        {
            return new LoadConditionalSetup<T>(_dbHookRegistrar, predicate);
        }
    }
}