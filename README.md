EntityFramework Hooks
==============

EntityHooks provides an extension points for EF Code First DbContext in order to hook database operations.
It is designed to be easy-to-use, unit-testable and IoC compatible.

## How to use

There are few ways to hook into DbContext load/save operations: 
- using a DbContextHooker class passing your DbContext instance into constructor;
- using fluent interface;
- deriving your DbContext from DbHookContext class. That option allows you to attach post-save hooks.

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
Setting date *Order* entity was modified:
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
