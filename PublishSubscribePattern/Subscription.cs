using System;

namespace PublishSubscribePattern {
    public class Subscription {
        readonly object handler;

        /// <summary>
        /// Gets the identifier of the subscription.
        /// </summary>
        /// <value>The identifier.</value>
        public Guid Id { get; }
        /// <summary>
        /// Gets the type of the event to subscribe to.
        /// </summary>
        /// <value>The type of the event.</value>
        public Type EventType { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:PublishSubscribePattern.Subscription"/> class.
        /// </summary>
        /// <param name="action">Action.</param>
        /// <param name="id">Identifier.</param>
        /// <param name="eventType">Event type.</param>
        public Subscription ( object action , Guid id , Type eventType ) {
            handler = action;
            Id = id;
            EventType = eventType;
        }

        /// <summary>
        /// Handle the specified event/message.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public void Handle<T> ( T message ) {
            if ( handler is Action<T> action ) {
                action ( message );
            }
        }
    }
}
