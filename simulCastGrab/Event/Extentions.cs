using simulCastGrab.Event.Interfaces;
using static simulCastGrab.Event.EventHook;
namespace simulCastGrab.Event
{
    /// <summary>
    /// Safe <see cref="CEvent"/> Invocation.
    /// </summary>
    public static class Extentions
    {
        /// <summary>
        /// Safely Invoke a <see cref="CEvent"/> of interface <see cref="IEvent"/>.
        /// </summary>
        /// <typeparam name="T">Event argument type.</typeparam>
        /// <param name="eh">Original Event.</param>
        /// <param name="arg">Event type argument.</param>
        public static void SafeInvoke<T>(this CEvent<T> eh, T arg) where T : IEvent
        {
            if (eh == null)
                return;
            eh.GetInvocationList()
                .ToList().ForEach(t =>
                {
                    try
                    {
                        ((CEvent<T>)t)(arg);
                    }
                    catch (Exception ex)
                    {
                        throw new CouldNotExecuteEventException($"{ex}: {t.Method.Name}, {eh.GetType().FullName}");
                    }
                });
        }

        /// <summary>
        /// Safely Invoke a <see cref="CEvent"/>.
        /// </summary>
        /// <param name="eh">Original Event.</param>
        public static void SafeInvoke(this CEvent eh)
        {
            if (eh == null)
                return;

            eh.GetInvocationList()
                .ToList().ForEach(t =>
                {
                    try
                    {
                        ((CEvent)t)();
                    }
                    catch (Exception ex)
                    {
                        throw new CouldNotExecuteEventException($"{ex}: {t.Method.Name}, {eh.GetType().FullName}");
                    }
                });
        }
    }
}
