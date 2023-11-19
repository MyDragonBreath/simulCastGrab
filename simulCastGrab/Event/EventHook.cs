using simulCastGrab.Events.Interfaces;

namespace simulCastGrab.Events
{
    /// <summary>
    /// <see cref="EventHook"/> Class reference.
    /// </summary>
    public static class EventHook
    {
        /// <summary>
        /// <see cref="CEvent"/> Action Ref.
        /// </summary>
        public delegate void CEvent();

        /// <summary>
        /// <see cref="CEvent"/> Action Ref.
        /// </summary>
        /// <typeparam name="TInterface"><see cref="CEvent{TInterface}"/> type.</typeparam>
        /// <param name="ev"><see cref="CEvent{TInterface}"/> instance.</param>
        public delegate void CEvent<TInterface>(TInterface ev) where TInterface : IEvent;
    }

    /// <summary>
    /// An Exception for the failed execution of an event.
    /// </summary>
    public class CouldNotExecuteEventException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouldNotExecuteEventException"/> class.
        /// </summary>
        /// <param name="message">The inserted message.</param>
        public CouldNotExecuteEventException(string message) : base(message)
        {
        }
    }
}
