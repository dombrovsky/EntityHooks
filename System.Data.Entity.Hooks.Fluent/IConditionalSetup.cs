namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Setup for a hook, that should be called when entity satisfies specified condition.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    public interface IConditionalSetup<out T> : IInvokeSetup<T> where T : class
    {
        /// <summary>
        /// Spicifies the setup for a hook, that should be called when entity satisfies specified condition.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <returns>
        /// The setup for a hook.
        /// </returns>
        IConditionalSetup<T> And(Predicate<T> predicate);
    }
}