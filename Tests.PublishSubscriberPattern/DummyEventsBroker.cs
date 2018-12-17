using System.Collections.Generic;

using PublishSubscribePattern;

namespace Tests.PublishSubscriberPattern {
    internal class DummyEventsBroker : EventsBroker {
        internal List<Subscription> Subscriptions => subscriptions;
    }
}
