using System;
using System.Threading.Tasks;

namespace PublishSubscribePattern
{
    internal interface IScheduler
    {
        Task Enqueue(Func<Task> task);
    }
}