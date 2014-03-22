namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Immutable adapter for <see cref="IDbEntityEntry"/>.
    /// </summary>
    internal sealed class EntityEntryFreezedAdapter : IDbEntityEntry
    {
        private readonly object _entity;

        private readonly EntityState _entityState;

        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEntryFreezedAdapter"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public EntityEntryFreezedAdapter(IDbEntityEntry entry)
        {
            _entity = entry.Entity;
            _entityState = entry.State;
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
            get { return _entityState; }
        }
    }
}
