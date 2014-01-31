using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// An Entity Framework DbContext that provides possibility to hook load and save actions.
    /// </summary>
    public abstract class DbHookContext : DbContext
    {
        private readonly List<IDbHook> _loadHooks;
        private readonly List<IDbHook> _preSaveHooks;
        private readonly List<IDbHook> _postSaveHooks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbHookContext"/> class.
        /// </summary>
        protected DbHookContext()
        {
            _loadHooks = new List<IDbHook>();
            _preSaveHooks = new List<IDbHook>();
            _postSaveHooks = new List<IDbHook>();

            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database and executes pre/post save hooks.
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().ToArray();

            foreach (var entry in entries)
            {
                foreach (var preSaveHook in _preSaveHooks)
                {
                    preSaveHook.HookEntry(entry);
                }
            }

            try
            {
                return base.SaveChanges();
            }
            finally
            {
                foreach (var entry in entries)
                {
                    foreach (var postSaveHook in _postSaveHooks)
                    {
                        postSaveHook.HookEntry(entry);
                    }
                }
            }
        }

        /// <summary>
        /// Registers a hook to run on object materialization stage.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        protected void RegisterLoadHook(IDbHook dbHook)
        {
            _loadHooks.Add(dbHook);
        }

        /// <summary>
        /// Registers a hook to run before save data occurs.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        protected void RegisterPreSaveHook(IDbHook dbHook)
        {
            _preSaveHooks.Add(dbHook);
        }

        /// <summary>
        /// Registers a hook to run after save data occurs.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        protected void RegisterPostSaveHook(IDbHook dbHook)
        {
            _postSaveHooks.Add(dbHook);
        }

        private void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            foreach (var loadHook in _loadHooks)
            {
                loadHook.HookEntry(Entry(e.Entity));
            }
        }
    }
}
