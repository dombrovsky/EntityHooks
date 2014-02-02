namespace System.Data.Entity.Hooks.Fluent.Internal
{
    /// <summary>
    /// Setup for a hook, that should be called when entity satisfies specified condition.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    internal abstract class ConditionalSetup<T> : IConditionalSetup<T> where T : class
    {
        private readonly EntityState _entityState;
        private readonly Predicate<T> _predicate;
        private readonly IDbHookRegistrar _dbHookRegistrar;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConditionalSetup{T}"/> class.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="entityState">State of the entity.</param>
        protected ConditionalSetup(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState)
        {
            _dbHookRegistrar = dbHookRegistrar;
            _predicate = predicate;
            _entityState = entityState;
        }

        /// <summary>
        /// Sets the action to be invoked by hook.
        /// </summary>
        /// <param name="hookAction">The hook action.</param>
        public void Do(Action<T> hookAction)
        {
            var hook = new DbHook<T>(
                obj =>
                    {
                        if (_predicate(obj))
                        {
                            hookAction(obj);
                        }
                    },
                _entityState);

            RegisterHook(_dbHookRegistrar, hook);
        }

        /// <summary>
        /// Spicifies the setup for a hook, that should be called when entity satisfies specified condition.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        public IConditionalSetup<T> And(Predicate<T> predicate)
        {
            return OnAnd(_dbHookRegistrar, obj => _predicate(obj) && predicate(obj), _entityState);
        }

        /// <summary>
        /// Called when one more condition is set up.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="entityState">State of the entity.</param>
        /// <returns>The setup for a hook.</returns>
        protected abstract IConditionalSetup<T> OnAnd(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState);

        /// <summary>
        /// Registers the hook.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <param name="hook">The hook.</param>
        protected abstract void RegisterHook(IDbHookRegistrar dbHookRegistrar, IDbHook hook);
    }
}