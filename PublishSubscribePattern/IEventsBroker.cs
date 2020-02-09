using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PublishSubscribePattern.Contracts
{
    public interface IEventsBroker
    {
        /// <summary>
        /// Subscribe to a certain message type T with a function to be done once the message is launched.
        /// It returns a Guid Id to uniquely identify the subscription.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="func"></param>
        /// <returns></returns>
        Task<Guid> SubscribeTo<T>(Func<T, Task> func, string channelName = ChannelName.Default);

        /// <summary>
        /// Remove the subscription based on it's id in the specified channel.
        /// </summary>
        /// <param name="id">Id of the subscription</param>
        /// <param name="channelName">Name of the channel from which to remove the subscription</param>
        /// <returns>A bool indicating if the operation has succeeded or not.</returns>
        Task<bool> Unsubscribe(Guid id, string channelName = ChannelName.Default);

        /// <summary>
        /// Publishes a message on a channel and launches the subscriptions sequentially.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        Task Publish<T>(T message, string channelName = ChannelName.Default, bool asParallel = false);
    }
}
