namespace System.Data.Entity.Hooks.Fluent
{
    /// <summary>
    /// Fluent API entry point.
    /// </summary>
    public interface IHookSetup : IFluentInterface
    {
        /// <summary>
        /// Spicifies the setup for a hook, that should be called while saving entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <returns>The setup for a hook.</returns>
        ISaveSetup<T> OnSave<T>() where T : class;

        /// <summary>
        /// Spicifies the setup for a hook, that should be called while loading entity of specified type.
        /// </summary>
        /// <typeparam name="T">Type of the entity.</typeparam>
        /// <returns>The setup for a hook.</returns>
        ILoadSetup<T> OnLoad<T>() where T : class;
    }
}
