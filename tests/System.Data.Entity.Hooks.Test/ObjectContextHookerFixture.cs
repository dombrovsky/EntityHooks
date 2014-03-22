using System.Data.Entity.Infrastructure;

namespace System.Data.Entity.Hooks.Test
{
    internal sealed class ObjectContextHookerFixture : DbContextHookerFixture
    {
        protected override DbContextHooker CreateContextHooker(DbContext dbContext)
        {
            return new DbContextHooker(((IObjectContextAdapter)dbContext).ObjectContext);
        }
    }
}