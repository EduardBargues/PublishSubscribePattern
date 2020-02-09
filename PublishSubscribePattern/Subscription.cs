using System;
using System.Threading.Tasks;

namespace PublishSubscribePattern
{
    public class Subscription
    {
        readonly object handler;

        /// <summary>
        /// Gets the type of the event to subscribe to.
        /// </summary>
        /// <value>The type of the event.</value>
        public Type EventType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Subscription"/> class.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="id">Identifier.</param>
        /// <param name="eventType">Event type.</param>
        internal Subscription(object action, Type type)
        {
            handler = action;
            EventType = type;
        }

        internal Subscription()
        {

        }

        /// <summary>
        /// Handle the specified event/message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public async Task Handle<T>(T message)
        {
            if (handler is Func<T, Task> func)
                await func(message).ConfigureAwait(false);
        }
    }
}
