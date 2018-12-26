using System;
using System.Collections.Concurrent;

using PublishSubscribePattern;

namespace Test.PublishSubscribePattern {
    internal class DummyEventsBroker : EventsBroker {
        internal ConcurrentDictionary<Guid, Subscription> Subscriptions => subscriptions;
        internal ConcurrentDictionary<Guid, Subscription> GeneralSubscriptions => generalSubscriptions;
    }
}
