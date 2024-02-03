namespace Yaga
{
    /// <summary>
    /// Represents empty type with only one instance.
    /// </summary>
    public struct Unit
    {
        public static Unit Instance { get; } = new Unit();
    }
}