using simulCastGrab.Services;

namespace simulCastGrab.Events.Interfaces
{
    /// <summary>
    /// Interface for all other event argument reference types.
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// Gets a platform indicator.
        /// </summary>
        public Platforms Platform { get; }
    }
}
