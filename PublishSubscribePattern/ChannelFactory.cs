using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Nito.AsyncEx;
using PublishSubscribePattern.Abstractions;

namespace PublishSubscribePattern
{
    internal class ChannelFactory : IChannelFactory
    {
        readonly Dictionary<string, IChannel> channelsByName;
        readonly ILogger logger;

        public ChannelFactory(ILogger logger, IOptions<ChannelFactoryConfiguration> configurationOptions)
        {
            this.logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            Guard.Argument(configurationOptions, nameof(configurationOptions)).NotNull();
            Log($"Creating channel factory.");
            channelsByName = configurationOptions.Value.ChannelNames
                .Select(channelName => (channelName,
                                        channel: Channel.NewChannel(channelName, logger)))
                .ToDictionary(t => t.channelName,
                              t => t.channel);
        }

        public IChannel GetChannel(string channelName)
        {
            Log($"Getting channel {channelName}");
            return channelsByName[channelName];
        }

        void Log(string message) => logger.LogDebug($"{nameof(ChannelFactory)} - {message}");
    }
}
