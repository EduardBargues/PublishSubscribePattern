using System;
using System.Threading.Tasks;

namespace PublishSubscribePattern
{
    internal interface IChannel
    {
        Task<Guid> Subscribe(Subscription subscription);
        Task<bool> Unsubscribe(Guid id);
        Task Publish<T>(T message);
    }
}