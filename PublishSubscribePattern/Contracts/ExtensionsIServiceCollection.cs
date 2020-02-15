using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PublishSubscribePattern.Contracts;

namespace PublishSubscribePattern
{
    public static class ExtensionsIServiceCollection
    {
        public static IServiceCollection AddEventsBroker(this IServiceCollection services)
        {
            return services
                .AddSingleton<IEventsBroker, EventsBroker>()
                .AddSingleton<IChannelFactory, ChannelFactory>();
        }
    }
}
