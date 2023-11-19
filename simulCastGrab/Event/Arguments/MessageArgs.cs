using simulCastGrab.Events.Interfaces;
using simulCastGrab.Services;

namespace simulCastGrab.Events.Arguments
{
    /// <summary>
    /// Argument for a basic message event.
    /// </summary>
    public class MessageArgs : IEvent
    {
        /// <summary>
        /// Gets the Username.
        /// </summary>
        public string Username { get; }

        /// <summary>
        /// Gets the Message.
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Gets the User's Id.
        /// </summary>
        public string Identifier { get; }

        /// <summary>
        /// Gets the platform from the User.
        /// </summary>
        public Platforms Platform { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageArgs"/> class.
        /// </summary>
        /// <param name="username">Username.</param>
        /// <param name="message">Message.</param>
        /// <param name="identifier">User's Id.</param>
        /// <param name="platform">User's Platform.</param>
        public MessageArgs(string username, string message, string identifier, Platforms platform)
        {
            Username = username;
            Message = message;
            Identifier = identifier;
            Platform = platform;
        }
    }
}
