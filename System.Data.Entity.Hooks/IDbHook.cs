using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Database action hook.
    /// </summary>
    public interface IDbHook
    {
        /// <summary>
        /// Hooks the entity entry.
        /// </summary>
        /// <param name="entry">The entity entry.</param>
        void HookEntry(DbEntityEntry entry);
    }
}
