using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Adapter for <see cref="DbEntityEntry"/> class.
    /// </summary>
    internal sealed class DbEntityEntryAdapter : IDbEntityEntry
    {
        private readonly DbEntityEntry _entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbEntityEntryAdapter"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public DbEntityEntryAdapter(DbEntityEntry entry)
        {
            _entry = entry;
        }

        /// <summary>
        /// Gets the entity.
        /// </summary>
        public object Entity
        {
            get { return _entry.Entity; }
        }

        /// <summary>
        /// Gets the state.
        /// </summary>
        public EntityState State
        {
            get { return _entry.State; }
        }
    }
}
