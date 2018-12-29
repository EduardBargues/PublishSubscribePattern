using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using MoreLinq;

namespace PublishSubscribePattern {
    public class EventsBroker : IEventsBroker {
        protected readonly ConcurrentDictionary<Guid,Subscription> subscriptions = new ConcurrentDictionary<Guid,Subscription> ();
        protected readonly ConcurrentDictionary<Guid,Subscription> generalSubscriptions = new ConcurrentDictionary<Guid,Subscription> ();

        /// <summary>
        /// Subscribe to a certain message type T with an action to be done once the message is launched.
        /// It returns a Guid Id to uniquely identify the subscription.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        public Guid SubscribeTo<T> ( Action<T> action ) {
            Subscription subscription = new Subscription ( action, Guid.NewGuid ( ), typeof ( T ) );
            bool ok = subscriptions.TryAdd ( subscription.Id, subscription );
            return ok ? subscription.Id : Guid.Empty;
        }
        /// <summary>
        /// Adds a subscription that will be launched for every published message.
        /// Useful when you want to add a log action to monitor every message.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public Guid SubscribeToAll ( Action<object> action ) {
            Subscription subscription = new Subscription ( action, Guid.NewGuid ( ), typeof ( object ) );
            bool ok = generalSubscriptions.TryAdd ( subscription.Id, subscription );
            return ok ? subscription.Id : Guid.Empty;
        }
        /// <summary>
        /// Checks in the general subscriptions and standard subscriptions if a subscription is present 
        /// based on it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsSubscribed ( Guid id ) => generalSubscriptions.ContainsKey ( id ) ||
                    subscriptions.ContainsKey ( id );
        /// <summary>
        /// Remove the subscription based on it's id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Unsubscribe ( Guid id ) {
            bool ok = generalSubscriptions.TryRemove ( id, out Subscription _ ) ||
                subscriptions.TryRemove ( id, out Subscription _ );
            return ok;
        }
        /// <summary>
        /// Publishes a message and launches the subscription sequentially by default.
        /// They can also be launched in parallel using the input asParallel.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="message"></param>
        /// <param name="asParallel"></param>
        public void Publish<T> ( T message, bool asParallel = false ) {
            void Action ( Subscription s ) {
                if ( generalSubscriptions.ContainsKey ( s.Id ) ) {
                    s.Handle ( ( object ) message );
                } else {
                    s.Handle ( message );
                }
            }
            IEnumerable<Subscription> subs = generalSubscriptions.Values
                .Concat ( subscriptions.Values
                            .Where ( s => s.EventType == typeof ( T ) ) );
            if ( asParallel ) {
                subs
                    .AsParallel ( )
                    .ForAll ( Action );
            } else {
                subs.ForEach ( Action );
            }
        }
        /// <summary>
        /// Removes all subscriptions.
        /// </summary>
        public void ClearSubscriptions ( ) {
            generalSubscriptions.Clear ( );
            subscriptions.Clear ( );
        }
    }
}