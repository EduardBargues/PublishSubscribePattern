using System;

namespace PublishSubscribePattern {
    public class Subscription {
        readonly object handler;

        public Guid Id { get; }
        public Type EventType { get; }

        public Subscription ( object action , Guid id , Type eventType ) {
            handler = action;
            Id = id;
            EventType = eventType;
        }

        public void Handle<T> ( T message ) {
            if ( handler is Action<T> action ) {
                action ( message );
            }
        }
    }
}
