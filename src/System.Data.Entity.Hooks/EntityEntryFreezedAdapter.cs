namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Immutable adapter for <see cref="IDbEntityEntry"/>.
    /// </summary>
    internal sealed class EntityEntryFreezedAdapter : ImmutableEntityEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EntityEntryFreezedAdapter"/> class.
        /// </summary>
        /// <param name="entry">The entry.</param>
        public EntityEntryFreezedAdapter(IDbEntityEntry entry)
            : base(entry.Entity, entry.State)
        {
        }
    }
}
