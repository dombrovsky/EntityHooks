EntityFramework Hooks
==============

EntityHooks provides an extension points for EF Code First DbContext in order to hook database operations.
It is designed to be easy-to-use, unit-testable and IoC compatible.

There are two ways to hook into DbContext load/save operations: 
- using a DbContextHooker class passing your DbContext instance into constructor;
- deriving your DbContext from DbHookContext class. That option allows you to attach post-save hooks.
