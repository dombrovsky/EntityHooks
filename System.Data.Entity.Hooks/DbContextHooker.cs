using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// Provides possibility to hook load and save actions of <see cref="DbContext"/>.
    /// </summary>
    public sealed class DbContextHooker : IDisposable
    {
        private readonly DbContext _dbContext;
        private readonly ObjectContext _objectContext;
        private readonly List<IDbHook> _loadHooks;
        private readonly List<IDbHook> _saveHooks;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextHooker"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public DbContextHooker(DbContext dbContext)
        {
            _loadHooks = new List<IDbHook>();
            _saveHooks = new List<IDbHook>();

            _dbContext = dbContext;
            _objectContext = ((IObjectContextAdapter)dbContext).ObjectContext;
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
        public void RegisterPreSaveHook(IDbHook dbHook)
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
            foreach (var entry in _dbContext.ChangeTracker.Entries())
            {
                foreach (var preSaveHook in _saveHooks)
                {
                    preSaveHook.HookEntry(entry);
                }
            }
        }

        private void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            foreach (var loadHook in _loadHooks)
            {
                loadHook.HookEntry(_dbContext.Entry(e.Entity));
            }
        }
    }
}
