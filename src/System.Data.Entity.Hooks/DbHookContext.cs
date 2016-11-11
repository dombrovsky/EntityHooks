using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
#if NET45
using System.Threading;
using System.Threading.Tasks;
#endif

namespace System.Data.Entity.Hooks
{
    /// <summary>
    /// An Entity Framework DbContext that provides possibility to hook load and save actions.
    /// </summary>
    public abstract class DbHookContext : DbContext, IDbHookRegistrar
    {
        private readonly List<IDbHook> _loadHooks = new List<IDbHook>();
        private readonly List<IDbHook> _preSaveHooks = new List<IDbHook>();
        private readonly List<IDbHook> _postSaveHooks = new List<IDbHook>();

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to
        ///             which a connection will be made.  The by-convention name is the full name (namespace + class name)
        ///             of the derived context class.
        ///             See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        protected DbHookContext()
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Constructs a new context instance using conventions to create the name of the database to
        ///             which a connection will be made, and initializes it from the given model.
        ///             The by-convention name is the full name (namespace + class name) of the derived context class.
        ///             See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        /// <param name="model">The model that will back this context. </param>
        protected DbHookContext(DbCompiledModel model)
            : base(model)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///             database to which a connection will be made.
        ///             See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string. </param>
        protected DbHookContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Constructs a new context instance using the given string as the name or connection string for the
        ///             database to which a connection will be made, and initializes it from the given model.
        ///             See the class remarks for how this is used to create a connection.
        /// 
        /// </summary>
        /// <param name="nameOrConnectionString">Either the database name or a connection string. </param><param name="model">The model that will back this context. </param>
        protected DbHookContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database.
        ///             The connection will not be disposed when the context is disposed if <paramref name="contextOwnsConnection"/>
        ///             is <c>false</c>.
        /// 
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context. </param><param name="contextOwnsConnection">If set to <c>true</c> the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
        ///             </param>
        protected DbHookContext(DbConnection existingConnection, bool contextOwnsConnection) 
            : base(existingConnection, contextOwnsConnection)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Constructs a new context instance using the existing connection to connect to a database,
        ///             and initializes it from the given model.
        ///             The connection will not be disposed when the context is disposed if <paramref name="contextOwnsConnection"/>
        ///             is <c>false</c>.
        /// 
        /// </summary>
        /// <param name="existingConnection">An existing connection to use for the new context. </param><param name="model">The model that will back this context. </param><param name="contextOwnsConnection">If set to <c>true</c> the connection is disposed when the context is disposed, otherwise the caller must dispose the connection.
        ///             </param>
        protected DbHookContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) 
            : base(existingConnection, model, contextOwnsConnection)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Constructs a new context instance around an existing ObjectContext.
        /// 
        /// </summary>
        /// <param name="objectContext">An existing ObjectContext to wrap with the new context. </param><param name="dbContextOwnsObjectContext">If set to <c>true</c> the ObjectContext is disposed when the DbContext is disposed, otherwise the caller must dispose the connection.
        ///             </param>
        protected DbHookContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            ((IObjectContextAdapter)this).ObjectContext.ObjectMaterialized += ObjectMaterialized;
        }

        /// <summary>
        /// Saves all changes made in this context to the underlying database and executes pre/post save hooks.
        /// 
        /// </summary>
        /// 
        /// <returns>
        /// The number of state entries written to the underlying database. This can include
        ///             state entries for entities and/or relationships. Relationship state entries are created for
        ///             many-to-many relationships and relationships where there is no foreign key property
        ///             included in the entity class (often referred to as independent associations).
        /// 
        /// </returns>
        /// <exception cref="T:System.Data.Entity.Infrastructure.DbUpdateException">An error occurred sending updates to the database.</exception><exception cref="T:System.Data.Entity.Infrastructure.DbUpdateConcurrencyException">A database command did not affect the expected number of rows. This usually indicates an optimistic
        ///             concurrency violation; that is, a row has been changed in the database since it was queried.
        ///             </exception><exception cref="T:System.Data.Entity.Validation.DbEntityValidationException">The save was aborted because validation of entity property values failed.
        ///             </exception><exception cref="T:System.NotSupportedException">An attempt was made to use unsupported behavior such as executing multiple asynchronous commands concurrently
        ///             on the same context instance.</exception><exception cref="T:System.ObjectDisposedException">The context or connection have been disposed.</exception><exception cref="T:System.InvalidOperationException">Some error occurred attempting to process entities in the context either before or after sending commands
        ///             to the database.
        ///             </exception>
        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries().Select(entry => new DbEntityEntryAdapter(entry)).ToArray();

            foreach (var entry in entries)
            {
                foreach (var preSaveHook in _preSaveHooks)
                {
                    preSaveHook.HookEntry(entry);
                }
            }

            var freezedEntries = entries.Select(adapter => adapter.AsFreezed()).ToArray();

            try
            {
                BeforeSaveChanges();

                var rowsAffected = base.SaveChanges();

                AfterSaveChanges(rowsAffected);

                return rowsAffected;
            }
            finally
            {
                foreach (var entry in freezedEntries)
                {
                    foreach (var postSaveHook in _postSaveHooks)
                    {
                        postSaveHook.HookEntry(entry);
                    }
                }
            }
        }
#if NET45
        /// <summary>
        /// Asynchronously saves all changes made in this context to the underlying database and executes pre/post save hooks.
        /// 
        /// </summary>
        /// 
        /// <remarks>
        /// Multiple active operations on the same context instance are not supported.  Use 'await' to ensure
        ///             that any asynchronous operations have completed before calling another method on this context.
        /// 
        /// </remarks>
        /// <param name="cancellationToken">A <see cref="T:System.Threading.CancellationToken"/> to observe while waiting for the task to complete.
        ///             </param>
        /// <returns>
        /// A task that represents the asynchronous save operation.
        ///             The task result contains the number of state entries written to the underlying database. This can include
        ///             state entries for entities and/or relationships. Relationship state entries are created for
        ///             many-to-many relationships and relationships where there is no foreign key property
        ///             included in the entity class (often referred to as independent associations).
        /// 
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">Thrown if the context has been disposed.</exception>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            var entries = ChangeTracker.Entries().Select(entry => new DbEntityEntryAdapter(entry)).ToArray();

            foreach (var entry in entries)
            {
                foreach (var preSaveHook in _preSaveHooks)
                {
                    preSaveHook.HookEntry(entry);
                }
            }

            var freezedEntries = entries.Select(adapter => adapter.AsFreezed()).ToArray();

            try
            {
                return await base.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            }
            finally
            {
                if (!cancellationToken.IsCancellationRequested)
                {
                    foreach (var entry in freezedEntries)
                    {
                        foreach (var postSaveHook in _postSaveHooks)
                        {
                            postSaveHook.HookEntry(entry);
                        }
                    }
                }
            }
        }
#endif
        
        /// <summary>
        /// Disposes the context. The underlying <see cref="T:System.Data.Entity.Core.Objects.ObjectContext"/> is also disposed if it was created
        ///             is by this context or ownership was passed to this context when this context was created.
        ///             The connection to the database (<see cref="T:System.Data.Common.DbConnection"/> object) is also disposed if it was created
        ///             is by this context or ownership was passed to this context when this context was created.
        ///             Hooks are cleared.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            try
            {
                base.Dispose(disposing);
            }
            finally 
            {
                if (disposing)
                {
                    _loadHooks.Clear();
                    _preSaveHooks.Clear();
                    _postSaveHooks.Clear();
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

        /// <summary>
        /// Called right before SaveChanges method of the DbContext is called, and after pre save hooks are executed.
        /// </summary>
        protected virtual void BeforeSaveChanges()
        {
        }

        /// <summary>
        /// Called right after SaveChanges method of the DbContext is called, and before post save hooks are executed.
        /// </summary>
        /// <param name="rowsAffected">The number of objects written to the underlying database.</param>
        protected virtual void AfterSaveChanges(int rowsAffected)
        {
        }

        private void ObjectMaterialized(object sender, ObjectMaterializedEventArgs e)
        {
            ObjectStateEntry objectStateEntry;
            if (((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.TryGetObjectStateEntry(e.Entity, out objectStateEntry))
            {
                var entry = new ObjectStateEntryAdapter(objectStateEntry);
                foreach (var loadHook in _loadHooks)
                {
                    loadHook.HookEntry(entry);
                }
            }
            else
            {
                var entry = new ImmutableEntityEntry(e.Entity, EntityState.Detached);
                foreach (var loadHook in _loadHooks)
                {
                    loadHook.HookEntry(entry);
                }
            }
        }

        #region IDbHookRegistrar

        /// <summary>
        /// Registers a hook to run before save data occurs.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        void IDbHookRegistrar.RegisterSaveHook(IDbHook dbHook)
        {
            RegisterPreSaveHook(dbHook);
        }

        /// <summary>
        /// Registers a hook to run on object materialization stage.
        /// </summary>
        /// <param name="dbHook">The hook to register.</param>
        void IDbHookRegistrar.RegisterLoadHook(IDbHook dbHook)
        {
            RegisterLoadHook(dbHook);
        }

        #endregion
    }
}
