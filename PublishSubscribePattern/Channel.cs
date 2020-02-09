using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MoreLinq;
using Nito.AsyncEx;

namespace PublishSubscribePattern
{
    public class Channel
    {
        readonly Dictionary<Guid, Subscription> subscriptionsById = new Dictionary<Guid, Subscription>();
        private readonly AsyncLock door = new AsyncLock();

        public async Task<Guid> Subscribe(Subscription subscription)
        {
            using (await door.LockAsync())
            {
                Guid id = Guid.NewGuid();
                subscriptionsById.Add(id, subscription);
                return id;
            }
        }

        public async Task<bool> Unsubscribe(Guid id)
        {
            using (await door.LockAsync())
            {
                return subscriptionsById.Remove(id);
            }
        }

        public async Task Publish<T>(T message, bool asParallel = false)
        {
            using (await door.LockAsync())
            {
                IEnumerable<Subscription> subscriptions = subscriptionsById.Values
                    .Where(subscription => subscription.EventType == typeof(T));
                if (asParallel)
                {
                    IEnumerable<Task> tasks = subscriptions.Select(subs => subs.Handle(message));
                    await Task.WhenAll(tasks).ConfigureAwait(false);
                }
                else
                {
                    foreach (Subscription subscription in subscriptions)
                        await subscription.Handle(message).ConfigureAwait(false);
                }
            }
        }
    }
}
