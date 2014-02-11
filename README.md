EntityFramework Hooks
==============

EntityHooks provides an extension points for EF Code First DbContext in order to hook database operations.
It is designed to be easy-to-use, unit-testable and IoC compatible.

## How to use

### Installation
Install [NuGet package](https://www.nuget.org/packages/EntityHooks/) from [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console):
```
PM> Install-Package EntityHooks
```

### Fluent interface
Reference *System.Data.Entity.Hooks.Fluent.dll* assembly and include namespace in order to use fluent hooking interface.
```csharp
using System.Data.Entity.Hooks.Fluent;
```
Logging of loading *Order* entities from datatbase:
```csharp
dbContext.CreateHook()
        .OnLoad<Order>()
        .When(order => order.Status != 0)
        .Do(order => _logger.Write("Loaded entity " + order.Id));
```
Setting date when *Order* entity has been modified:
```csharp
dbContext.CreateHook()
         .OnSave<Order>()
         .When(EntityState.Added | EntityState.Modified)
         .And(order => order.Status == 1)
         .Do(order => order.ModifiedDate = DateTime.UtcNow);
```
Delete *Order* entity if either *CustomerId* or *CategoryId* is *NULL*:
```csharp
dbContext.CreateHook()
         .OnSave<Order>()
         .When(order => order.CustomerId == null || order.CategoryId == null)
         .Do(order => dbContext.Set<Order>().Remove(order));
```
However, it is still possible to attach any *IDbHook* implementation through fluent interface:
```csharp
dbContext.OnLoad()
         .Attach(new MyFancyHook())
         .Attach(new CustomHook());
dbContext.OnSave()
         .Attach(new MyFancyHook());
```

## How hooks are called

- Load hooks are called in the order they were registered when [*ObjectContext.ObjectMaterialized*](http://msdn.microsoft.com/en-us/library/system.data.objects.objectcontext.objectmaterialized(v=vs.110).aspx) event occur.
- Save hooks are called in the order they were registered for each entry in [*DbChangeTracker*](http://msdn.microsoft.com/en-us/library/system.data.entity.infrastructure.dbchangetracker(v=vs.113).aspx) before actual saving.

Hook is any class, that implements *IDbHook* interface. 
```csharp
public interface IDbHook
{
        void HookEntry(IDbEntityEntry entry);
}
```
*IDbEntityEntry* provides information about entity and it's state ([*EntityState*](http://msdn.microsoft.com/en-us/library/system.data.entitystate(v=vs.110).aspx) enumeration).
As you may have noticed, *IDbHook* interface don't have neither information about type of entity, which hook should be applied to, nor predicate when to invoke hook. That means, that implementors of *IDbHook* interface should provide their own logic, if needed.

**NOTE:** Instead of implementing *IDbHook* you may use in-build generic ```DbHook<TEntity>``` class, which provides calls user-specified action for entities of *TEntity* type and specific state.

### Entity state mutability
Entity state, provided by *IDbEntityEntry* argument passed to *IDbHook.HookEntry* method, reflects current state for Load or Pre-save hooks. If hook modifies state of entity, all further invoked hooks for that entity called with new state. However, for Post-save hooks it reflects the state of entity right before saving changes, despite the fact that actual state in that case might be *Unchanged*.
