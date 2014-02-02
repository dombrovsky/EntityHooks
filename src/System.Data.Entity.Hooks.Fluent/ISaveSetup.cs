namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Setup for a hook, that should be called while saving entity of specified type.
    /// </summary>
    /// <typeparam name="T">Type of the entity.</typeparam>
    public interface ISaveSetup<out T> : ITypedHookSetup<T> where T : class
    {
        /// <summary>
        /// Spicifies the setup for a hook, that should be called while saving an entity with specific <see cref="EntityState"/>.
        /// </summary>
        /// <param name="entityState">State of the entity.</param>
        /// <returns>The setup for a hook.</returns>
        IConditionalSetup<T> When(EntityState entityState);
    }
}