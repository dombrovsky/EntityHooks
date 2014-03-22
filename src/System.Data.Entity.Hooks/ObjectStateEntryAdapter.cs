using System.Data.Entity.Core.Objects;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Adapter for <see cref="ObjectStateEntry"/> class.
    /// </summary>
    internal sealed class ObjectStateEntryAdapter : IDbEntityEntry
    {
        private readonly ObjectStateEntry _entry;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectStateEntryAdapter"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public ObjectStateEntryAdapter(ObjectStateEntry entry)
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

        /// <summary>
        /// Freezes further entity state changes.
        /// </summary>
        /// <returns>Freezed instance of <see cref="IDbEntityEntry"/></returns>.
        public IDbEntityEntry AsFreezed()
        {
            return new EntityEntryFreezedAdapter(this);
        }
    }
}
