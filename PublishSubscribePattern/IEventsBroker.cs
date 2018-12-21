using System;
using System.Threading.Tasks;

namespace PublishSubscribePattern
{
    public interface IEventsBroker
    {
        /// <summary>
        /// Subscribes to an specific event type.
        /// </summary>
        /// <returns>The to.</returns>
        /// <param name="action">Action.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task<Guid> SubscribeTo<T> (Action<T> action);

        /// <summary>
        /// Subscribes to all events. Useful for log things.
        /// </summary>
        /// <returns>The general.</returns>
        /// <param name="action">Action.</param>
        Task<Guid> SubscribeGeneral (Action<object> action);

        /// <summary>
        /// Checks if the events broker has a subscription defined by the id.
        /// </summary>
        /// <returns>The subscribed.</returns>
        /// <param name="id">Identifier.</param>
        Task<bool> IsSubscribed (Guid id);

        /// <summary>
        /// Unsubscribe the specified id.
        /// </summary>
        /// <returns>The unsubscribe.</returns>
        /// <param name="id">Identifier.</param>
        Task Unsubscribe (Guid id);

        /// <summary>
        /// Unsubscribes from an specific type of event.
        /// </summary>
        /// <returns>The from.</returns>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task UnsubscribeFrom<T> ();

        /// <summary>
        /// Publishes the specified event.
        /// </summary>
        /// <returns>The publish.</returns>
        /// <param name="message">Message.</param>
        /// <param name="asParallel">If set to <c>true</c> as parallel.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        Task Publish<T> (T message, bool asParallel = false);

        /// <summary>
        /// Clears the subscriptions.
        /// </summary>
        /// <returns>The subscriptions.</returns>
        Task ClearSubscriptions ();
    }
}
