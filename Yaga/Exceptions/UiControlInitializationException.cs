namespace Yaga.Exceptions
{
    /// <summary>
    /// Thrown if uninitialized <see cref="UiControl"/> singleton instance is being used.
    /// </summary>
    public class UiControlInitializationException : InitializationException
    {
        public UiControlInitializationException() : base(
            $"{nameof(UiControl)} was not initialized. Use InitializeSingleton before using.")
        {
        }
    }
}