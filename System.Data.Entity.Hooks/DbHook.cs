namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Database hook for entity type.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to hook.</typeparam>
    public class DbHook<TEntity> : IDbHook where TEntity : class
    {
        private readonly Action<TEntity> _hookAction;
        private readonly EntityState _hookEntityState;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHook{TEntity}"/> class.
        /// </summary>
        /// <param name="hookAction">The hook action.</param>
        /// <param name="hookEntityState">State of the hook entity.</param>
        public DbHook(Action<TEntity> hookAction, EntityState hookEntityState)
        {
            _hookAction = hookAction;
            _hookEntityState = hookEntityState;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHook{TEntity}"/> class.
        /// </summary>
        /// <param name="hookAction">The hook action.</param>
        public DbHook(Action<TEntity> hookAction)
            : this(hookAction, EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added)
        {
        }

        /// <summary>
        /// Hooks the entity entry.
        /// </summary>
        /// <param name="entry">The entity entry.</param>
        public void HookEntry(IDbEntityEntry entry)
        {
            var entity = entry.Entity as TEntity;
            if (entity != null && (_hookEntityState & entry.State) != 0)
            {
                _hookAction(entity);
            }
        }
    }
}
