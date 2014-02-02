namespace System.Data.Entity.Hooks.Fluent.Internal
{
    /// <summary>
    /// Setup for a hook, that should be called when entity satisfies specified condition.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    internal sealed class LoadConditionalSetup<T> : ConditionalSetup<T> where T : class
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LoadConditionalSetup{T}"/> class.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <param name="predicate">The predicate.</param>
        public LoadConditionalSetup(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate)
            : base(dbHookRegistrar, predicate, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added)
        {
        }

        /// <summary>
        /// Called when one more condition is set up.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <param name="predicate">The predicate.</param>
        /// <param name="entityState">State of the entity.</param>
        /// <returns>The setup for a hook.</returns>
        protected override IConditionalSetup<T> OnAnd(IDbHookRegistrar dbHookRegistrar, Predicate<T> predicate, EntityState entityState)
        {
            return new LoadConditionalSetup<T>(dbHookRegistrar, predicate);
        }

        /// <summary>
        /// Registers the hook.
        /// </summary>
        /// <param name="dbHookRegistrar">The database hook registrar.</param>
        /// <param name="hook">The hook.</param>
        protected override void RegisterHook(IDbHookRegistrar dbHookRegistrar, IDbHook hook)
        {
            dbHookRegistrar.RegisterLoadHook(hook);
        }
    }
}