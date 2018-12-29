using System;

namespace PublishSubscribePattern {
    public interface IEventsBroker {
        /// <summary>
        /// Subscribe to a certain message type T with an action to be done once the message is launched.
        /// It returns a Guid Id to uniquely identify the subscription.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        Guid SubscribeTo<T> ( Action<T> action );
        /// <summary>
        /// Adds a subscription that will be launched for every published message.
        /// Useful when you want to add a log action to monitor every message.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        Guid SubscribeToAll ( Action<object> action );
        /// <summary>
        /// Checks in the general subscriptions and standard subscriptions if a subscription is present 
        /// based on it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool IsSubscribed ( Guid id );
        /// <summary>
        /// Remove the subscription based on it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Unsubscribe ( Guid id );
        /// <summary>
        /// Publishes a message and launches the subscription sequentially by default.
        /// They can also be launched in parallel using the input asParallel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="asParallel"></param>
        void Publish<T> ( T message, bool asParallel = false );
        /// <summary>
        /// Removes all subscriptions.
        /// </summary>
        void ClearSubscriptions ( );
    }
}
