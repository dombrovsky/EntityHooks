namespace System.Data.Entity.Hooks.Fluent.Internal
{
    /// <summary>
    /// Setup for a hook, that should be called while saving entity of specified type.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    internal sealed class SaveSetup<T> : ISaveSetup<T> where T : class
    {
        private readonly IDbHookRegistrar _dbHookRegistrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="SaveSetup{T}"/> class.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        public SaveSetup(IDbHookRegistrar dbHookRegistrar)
        {
            _dbHookRegistrar = dbHookRegistrar;
        }

        /// <summary>
        /// Sets the action to be invoked by hook.
        /// </summary>
        /// <param name="hookAction">The hook action.</param>
        public void Do(Action<T> hookAction)
        {
            _dbHookRegistrar.RegisterSaveHook(new DbHook<T>(hookAction));
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
            return new SaveConditionalSetup<T>(_dbHookRegistrar, predicate, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added);
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while saving an entity with specific <see cref="EntityState"/>.
        /// </summary>
        /// <param name="entityState">State of the entity.</param>
        /// <returns>The setup for a hook.</returns>
        public IConditionalSetup<T> When(EntityState entityState)
        {
            return new SaveConditionalSetup<T>(_dbHookRegistrar, obj => true, entityState);
        }
    }
}