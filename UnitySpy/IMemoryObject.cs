namespace UnitySpy
{
    /// <summary>
    /// Represents an object in a process' memory.
    /// </summary>
    public interface IMemoryObject
    {
        /// <summary>
        /// Gets the <see cref="IAssemblyImage"/> to which the object belongs.
        /// </summary>
        IAssemblyImage Image { get; }
    }
}