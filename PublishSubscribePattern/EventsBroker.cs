using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq;
using PublishSubscribePattern.Contracts;

namespace PublishSubscribePattern
{
    public class EventsBroker : IEventsBroker
    {
        private readonly ConcurrentDictionary<string, Channel> channelsByName = new ConcurrentDictionary<string, Channel>();
        private readonly ILogger<EventsBroker> logger;

        public EventsBroker(ILogger<EventsBroker> logger) => this.logger = logger;

        /// <inheritdoc/>
        public async Task<Guid> SubscribeTo<T>(Func<T, Task> func, string channelName = ChannelName.Default)
        {
            Channel channel = GetOrAddChannel(channelName);
            Subscription subscription = new Subscription(func, typeof(T));
            Guid id = await channel.Subscribe(subscription).ConfigureAwait(false);

            logger?.LogInformation($"Asynchronous subscription {id} on type {typeof(T).Name} added.");

            return id;
        }

        /// <inheritdoc/>
        public async Task<bool> Unsubscribe(Guid id, string channelName = ChannelName.Default)
        {
            Channel channel = GetOrAddChannel(channelName);
            bool ok = await channel.Unsubscribe(id).ConfigureAwait(false);

            if (ok) logger?.LogInformation($"Subscription {id} removed.");
            else logger?.LogWarning($"Failed to remove subscription {id}.");

            return ok;
        }

        /// <inheritdoc/>
        public async Task Publish<T>(T message, string channelName = ChannelName.Default, bool asParallel = false)
        {
            logger?.LogInformation($"Publishing message {message} ...");

            Channel channel = GetOrAddChannel(channelName);
            await channel.Publish(message, asParallel).ConfigureAwait(false);
        }

        private Channel GetOrAddChannel(string channelName) => channelsByName.GetOrAdd(channelName, new Channel());
    }
}