namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Allows to attach custom <see cref="IDbHook"/> instances on entity load.
    /// </summary>
    public interface ILoadHookAttacher : IFluentInterface
    {
        /// <summary>
        /// Attaches the specified hook to be called for every entity load.
        /// </summary>
        /// <param name="hook">The hook.</param>
        /// <returns>This instance.</returns>
        ILoadHookAttacher Attach(IDbHook hook);
    }
}