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
    internal class Channel
    {
        readonly ILogger logger;
        readonly string channelName;
        readonly Dictionary<Guid, Subscription> subscriptionsById;
        readonly Scheduler scheduler;
        readonly AsyncLock door = new AsyncLock();

        public Channel(string channelName, ILogger logger)
        {
            this.logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            this.channelName = Guard.Argument(channelName, nameof(channelName))
                .NotNull()
                .NotEmpty().Value;
            Log($"Creating channel.");
            subscriptionsById = new Dictionary<Guid, Subscription>();
            scheduler = new Scheduler(channelName, logger);
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
            Log("Publishing.");
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
