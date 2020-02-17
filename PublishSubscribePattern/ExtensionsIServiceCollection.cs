using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using PublishSubscribePattern.Abstractions;

namespace PublishSubscribePattern
{
    public static class ExtensionsIServiceCollection
    {
        public static IServiceCollection AddEventsBroker(this IServiceCollection services, Action<ChannelFactoryConfiguration> action)
           => services
                .AddSingleton<IChannelFactory, ChannelFactory>()
                .AddSingleton<IEventsBroker, EventsBroker>()
                .Configure(action);
    }
}
