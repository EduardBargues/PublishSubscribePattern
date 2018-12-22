using System;

namespace PublishSubscribePattern {
    public interface IEventsBroker {
        /// <summary>
        /// Subscribes to an specific event type. If it can not subscribe, the returned Guid will be empty.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Guid SubscribeTo<T> ( Action<T> action );

        /// <summary>
        /// Subscribes to all events. Useful for log things. If it can not subscribe, the returned Guid will be empty.
        /// </summary>
        /// <returns>The general.</returns>
        /// <param name="action">Action.</param>
        Guid SubscribeToAll ( Action<object> action );

        /// <summary>
        /// Checks if the events broker has a subscription defined by the id.
        /// </summary>
        /// <returns>The subscribed.</returns>
        /// <param name="id">Identifier.</param>
        bool IsSubscribed ( Guid id );

        /// <summary>
        /// Unsubscribe the specified id.
        /// </summary>
        /// <returns>The unsubscribe.</returns>
        /// <param name="id">Identifier.</param>
        bool Unsubscribe ( Guid id );

        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <returns>The publish.</returns>
        /// <param name="message">Message.</param>
        /// <param name="asParallel">If set to <c>true</c> as parallel.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        void Publish<T> ( T message , bool asParallel = false );

        /// <summary>
        /// Clears the subscriptions.
        /// </summary>
        /// <returns>The subscriptions.</returns>
        void ClearSubscriptions ( );
    }
}
