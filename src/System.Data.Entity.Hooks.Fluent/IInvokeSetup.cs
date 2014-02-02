namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Setup for a hook, specifying the hook action.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IInvokeSetup<out T> : IFluentInterface where T : class
    {
        /// <summary>
        /// Sets the action to be invoked by hook.
        /// </summary>
        /// <param name="hookAction">The hook action.</param>
        void Do(Action<T> hookAction);
    }
}