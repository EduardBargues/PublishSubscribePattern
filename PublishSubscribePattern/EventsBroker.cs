using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using MoreLinq;

using Nito.AsyncEx;

namespace PublishSubscribePattern {
    public class EventsBroker : IEventsBroker {
        protected readonly List<Subscription> subscriptions = new List<Subscription> ();
        readonly AsyncLock mutex = new AsyncLock ();

        public async Task<Guid> SubscribeTo<T> ( Action<T> action ) {
            Subscription subscription = new Subscription ( action , Guid.NewGuid ( ) , typeof ( T ) );
            using ( await mutex.LockAsync ( ) ) {
                subscriptions.Add ( subscription );
            }
            return subscription.Id;
        }
        public async Task<bool> IsSubscribed ( Guid id ) {
            using ( await mutex.LockAsync ( ) ) {
                return subscriptions.Any ( s => s.Id == id );
            }
        }
        public async Task Unsubscribe ( Guid id ) {
            using ( await mutex.LockAsync ( ) ) {
                subscriptions.Remove ( subscriptions.FirstOrDefault ( s => s.Id == id ) );
            }
        }
        public async Task UnsubscribeFrom<T> ( ) {
            using ( await mutex.LockAsync ( ) ) {
                subscriptions.RemoveAll ( s => s.EventType == typeof ( T ) );
            }
        }
        public async Task Publish<T> ( T message , bool asParallel = false ) {
            void Action ( Subscription s ) => s.Handle ( message );
            using ( await mutex.LockAsync ( ) ) {
                IEnumerable<Subscription> subs = subscriptions
                    .Where ( s => s.EventType == typeof ( T ) );
                if ( asParallel ) {
                    subs.AsParallel ( ).ForAll ( Action );
                } else {
                    subs.ForEach ( Action );
                }
            }
        }
        public async Task ClearSubscriptions ( ) {
            using ( await mutex.LockAsync ( ) ) {
                subscriptions.Clear ( );
            }
        }
    }
}