namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown if uninitialized <see cref="UiBootstrap"/> singleton instance is being used.
    /// </summary>
    public class UiBootstrapInitializationException : InitializationException
    {
        public UiBootstrapInitializationException() : base(
            $"{nameof(UiBootstrap)} was not initialized. Use InitializeSingleton before using.")
        {
        }
    }
}