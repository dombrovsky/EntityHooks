using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Adapter for <see cref="DbEntityEntry"/> class.
    /// </summary>
    internal sealed class DbEntityEntryFreezedAdapter : IDbEntityEntry
    {
        private readonly object _entity;

        private readonly EntityState _entityState;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbEntityEntryFreezedAdapter"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public DbEntityEntryFreezedAdapter(DbEntityEntry entry)
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
