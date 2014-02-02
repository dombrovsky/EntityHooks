namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Setup for a hook, that should be called while loading entity of specified type.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    public interface ILoadSetup<out T> : ITypedHookSetup<T> where T : class
    {
    }
}