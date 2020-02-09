using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PublishSubscribePattern.Contracts;

namespace PublishSubscribePattern
{
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureEventsBroker(this IServiceCollection services)
            => services.AddSingleton<IEventsBroker, EventsBroker>();
    }
}
