namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Allows to attach custom <see cref="IDbHook"/> instances on entity save.
    /// </summary>
    public interface ISaveHookAttacher : IFluentInterface
    {
        /// <summary>
        /// Attaches the specified hook to be called for every entity save.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns>This instance.</returns>
        ISaveHookAttacher Attach(IDbHook hook);
    }
}