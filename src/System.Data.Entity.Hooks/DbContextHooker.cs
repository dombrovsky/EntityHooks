using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Provides possibility to hook load and save actions of <see cref="DbContext"/>.
    /// </summary>
    public sealed class DbContextHooker : IDbHookRegistrar, IDisposable
    {
        private readonly ObjectContext _objectContext;
        private readonly List<IDbHook> _loadHooks;
        private readonly List<IDbHook> _saveHooks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextHooker"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public DbContextHooker(DbContext dbContext)
            : this(((IObjectContextAdapter)dbContext).ObjectContext)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextHooker"/> class.
        /// </summary>
        /// <param name="objectContext">The object context.</param>
        public DbContextHooker(ObjectContext objectContext)
        {
            _loadHooks = new List<IDbHook>();
            _saveHooks = new List<IDbHook>();

            _objectContext = objectContext;
            _objectContext.ObjectMaterialized += ObjectMaterialized;
            _objectContext.SavingChanges += SavingChanges;
        }

        /// <summary>
        /// Registers a hook to run on object materialization stage.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        public void RegisterLoadHook(IDbHook dbHook)
        {
            _loadHooks.Add(dbHook);
        }

        /// <summary>
        /// Registers a hook to run before save data occurs.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        public void RegisterSaveHook(IDbHook dbHook)
        {
            _saveHooks.Add(dbHook);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _objectContext.ObjectMaterialized -= ObjectMaterialized;
            _objectContext.SavingChanges -= SavingChanges;
        }

        private void SavingChanges(object sender, EventArgs e)
        {
            var entries = _objectContext.ObjectStateManager
                .GetObjectStateEntries(EntityState.Unchanged | EntityState.Modified | EntityState.Deleted | EntityState.Added)
                .Select(entry => new ObjectStateEntryAdapter(entry));

            foreach (var entry in entries)
            {
                foreach (var preSaveHook in _saveHooks)
                {
                    preSaveHook.HookEntry(entry);
                }
            }
        }

        private void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            var entry = new ObjectStateEntryAdapter(_objectContext.ObjectStateManager.GetObjectStateEntry(e.Entity));
            foreach (var loadHook in _loadHooks)
            {
                loadHook.HookEntry(entry);
            }
        }
    }
}
