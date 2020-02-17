using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using MoreLinq;
using Nito.AsyncEx;

namespace PublishSubscribePattern
{
    internal class Channel : IChannel
    {
        readonly ILogger logger;
        readonly string channelName;
        protected readonly Dictionary<Guid, Subscription> subscriptionsById;
        readonly IScheduler scheduler;
        readonly AsyncLock door = new AsyncLock();

        public static IChannel NewChannel(string channelName, ILogger logger)
            => new Channel(channelName, logger, new Scheduler($"Scheduler {channelName}", logger));

        public Channel() { }
        public Channel(string channelName, ILogger logger, IScheduler scheduler)
        {
            this.logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            this.channelName = Guard.Argument(channelName, nameof(channelName))
                .NotNull()
                .NotEmpty().Value;
            this.scheduler = Guard.Argument(scheduler, nameof(scheduler)).NotNull().Value;
            Log($"Creating channel.");
            subscriptionsById = new Dictionary<Guid, Subscription>();
            door = new AsyncLock();
        }

        public async Task<Guid> Subscribe(Subscription subscription)
        {
            using (await door.LockAsync())
            {
                Guid id = Guid.NewGuid();
                Log($"Subscribing {id}.");
                subscriptionsById.Add(id, subscription);
                return id;
            }
        }

        public async Task<bool> Unsubscribe(Guid id)
        {
            Log($"Unsubscribing {id}.");
            using (await door.LockAsync())
                return subscriptionsById.Remove(id);
        }

        public async Task Publish<T>(T message)
        {
            Log($"Publishing {message}.");
            using (await door.LockAsync())
            {
                List<Subscription> subscriptions = subscriptionsById.Values
                    .Where(subscription => subscription.EventType == typeof(T))
                    .ToList();
                await scheduler.Enqueue(() => Handle(message, subscriptions)).ConfigureAwait(false);
            }
        }

        static async Task Handle<T>(T message, List<Subscription> subscriptions)
        {
            foreach (Subscription subscription in subscriptions)
                await subscription.Handle(message).ConfigureAwait(false);
        }

        void Log(string message) => logger.LogDebug($"{nameof(Channel)} {channelName} - {message}");
    }
}
