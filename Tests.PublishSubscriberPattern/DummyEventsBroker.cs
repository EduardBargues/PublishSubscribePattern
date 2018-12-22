using System;
using System.Collections.Concurrent;

using PublishSubscribePattern;

namespace Tests.PublishSubscriberPattern {
    internal class DummyEventsBroker : EventsBroker {
        internal ConcurrentDictionary<Guid , Subscription> Subscriptions => subscriptions;
    }
}
