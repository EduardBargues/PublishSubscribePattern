using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MoreLinq;
using PublishSubscribePattern.Abstractions;
[assembly: InternalsVisibleTo("PublishSubscribePattern.Tests")]
namespace PublishSubscribePattern
{
    internal class EventsBroker : IEventsBroker
    {
        readonly IChannelFactory channelFactory;

        public EventsBroker(IChannelFactory channelFactory)
        {
            this.channelFactory = Guard.Argument(channelFactory, nameof(channelFactory)).NotNull().Value;
        }

        /// <inheritdoc/>
        public async Task<Guid> Subscribe<T>(Func<T, Task> func, string channelName = ChannelName.Default)
        {
            Guard.Argument(channelName, nameof(channelName)).NotNull().NotEmpty();

            IChannel channel = channelFactory.GetChannel(channelName);
            Subscription subscription = new Subscription(func, typeof(T));
            Guid id = await channel.Subscribe(subscription).ConfigureAwait(false);

            return id;
        }

        /// <inheritdoc/>
        public async Task<bool> Unsubscribe(Guid id, string channelName = ChannelName.Default)
        {
            IChannel channel = channelFactory.GetChannel(channelName);
            return await channel.Unsubscribe(id).ConfigureAwait(false);
        }

        /// <inheritdoc/>
        public async Task Publish<T>(T message, string channelName = ChannelName.Default)
        {
            IChannel channel = channelFactory.GetChannel(channelName);
            await channel.Publish(message).ConfigureAwait(false);
        }
    }
}