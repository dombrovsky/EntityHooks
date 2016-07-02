namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Immutable adapter for <see cref="IDbEntityEntry"/>.
    /// </summary>
    internal class ImmutableEntityEntry : IDbEntityEntry
    {
        private readonly object _entity;
        private readonly EntityState _state;

        /// <summary>
        /// Initializes a new instance of the <see cref="ImmutableEntityEntry"/> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="state">The state.</param>
        public ImmutableEntityEntry(object entity, EntityState state)
        {
            _entity = entity;
            _state = state;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public object Entity
        {
            get { return _entity; }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public EntityState State
        {
            get { return _state; }
        }
    }
}