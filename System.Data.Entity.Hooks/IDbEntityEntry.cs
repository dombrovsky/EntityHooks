namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Provide access to information about and control of entities that are being tracked by the <see cref="T:System.Data.Entity.DbContext"/>.
    /// </summary>
    public interface IDbEntityEntry
    {
        /// <summary>
        /// Gets the entity.
        /// </summary>
        object Entity { get; }

        /// <summary>
        /// Gets the state.
        /// </summary>
        EntityState State { get; }
    }
}
