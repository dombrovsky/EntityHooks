using System.Collections.Generic;
using System.Data.Common;
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
        /// </summary>
        /// <returns>The number of objects written to the underlying database.</returns>
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
            var entry = new DbEntityEntryAdapter(Entry(e.Entity));
            foreach (var loadHook in _loadHooks)
            {
                loadHook.HookEntry(entry);
            }
        }
    }
}
