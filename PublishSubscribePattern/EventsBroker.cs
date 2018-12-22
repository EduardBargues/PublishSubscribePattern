using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

using MoreLinq;

namespace PublishSubscribePattern {
    public class EventsBroker : IEventsBroker {
        protected readonly ConcurrentDictionary<Guid,Subscription> subscriptions = new ConcurrentDictionary<Guid,Subscription> ();
        protected readonly ConcurrentDictionary<Guid,Subscription> generalSubscriptions = new ConcurrentDictionary<Guid,Subscription> ();

        public Guid SubscribeTo<T> ( Action<T> action ) {
            Subscription subscription = new Subscription ( action , Guid.NewGuid ( ) , typeof ( T ) );
            bool ok = subscriptions.TryAdd ( subscription.Id , subscription );
            return ok ? subscription.Id : Guid.Empty;
        }

        public Guid SubscribeToAll ( Action<object> action ) {
            Subscription subscription = new Subscription ( action , Guid.NewGuid ( ) , typeof ( object ) );
            bool ok = generalSubscriptions.TryAdd ( subscription.Id , subscription );
            return ok ? subscription.Id : Guid.Empty;
        }

        public bool IsSubscribed ( Guid id ) => generalSubscriptions.ContainsKey ( id ) ||
                    subscriptions.ContainsKey ( id );

        public bool Unsubscribe ( Guid id ) {
            bool ok = generalSubscriptions.TryRemove ( id , out Subscription _ ) ||
                subscriptions.TryRemove ( id , out Subscription _ );
            return ok;
        }

        public void Publish<T> ( T message , bool asParallel = false ) {
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

        public void ClearSubscriptions ( ) {
            generalSubscriptions.Clear ( );
            subscriptions.Clear ( );
        }
    }
}