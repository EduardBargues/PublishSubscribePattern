using System;
using System.Threading.Tasks;

namespace PublishSubscribePattern {
    public interface IEventsBroker {
        Task<Guid> SubscribeTo<T> ( Action<T> action );
        Task<bool> IsSubscribed ( Guid id );
        Task Unsubscribe ( Guid id );
        Task UnsubscribeFrom<T> ( );
        Task Publish<T> ( T message , bool asParallel = false );
        Task ClearSubscriptions ( );
    }
}
