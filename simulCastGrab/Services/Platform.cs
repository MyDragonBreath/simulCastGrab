using simulCastGrab.Events;
using simulCastGrab.Events.Arguments;
using static simulCastGrab.Events.EventHook;

namespace simulCastGrab.Services
{
    /// <summary>
    /// A basic platform service.
    /// </summary>
    public abstract class Platform
    {
        /// <summary>
        /// Gets or Sets the OnMessage Event.
        /// </summary>
        public CEvent<MessageArgs>? OnMessage { get; set; }

        /// <summary>
        /// Invokes the OnMessage Event.
        /// </summary>
        /// <param name="args">A <see cref="MessageArgs"/> instance.</param>
        protected void InvokeOnMessage(MessageArgs args) => OnMessage?.SafeInvoke(args);

        /// <summary>
        /// Gets the platform enumerator value.
        /// </summary>
        public abstract Platforms platformE { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Platform"/> class.
        /// </summary>
        public Platform()
        {
        }
    }
}
