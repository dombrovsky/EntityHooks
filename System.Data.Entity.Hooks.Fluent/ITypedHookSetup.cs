namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Setup for a hook, that should be called for entity of specified type.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ITypedHookSetup<out T> : IInvokeSetup<T> where T : class
    {
        /// <summary>
        /// Spicifies the setup for a hook, that should be called when entity satisfies specified condition.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        IConditionalSetup<T> When(Predicate<T> predicate);
    }
}