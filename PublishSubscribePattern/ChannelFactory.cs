using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Dawn;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;

namespace PublishSubscribePattern
{
    internal class ChannelFactory : IChannelFactory
    {
        readonly Dictionary<string, Channel> channelsByName;
        readonly AsyncLock door;
        readonly ILogger<ChannelFactory> logger;

        public ChannelFactory(ILogger<ChannelFactory> logger)
        {
            this.logger = Guard.Argument(logger, nameof(logger)).NotNull().Value;
            Log($"Creating channel factory.");
            channelsByName = new Dictionary<string, Channel>();
            door = new AsyncLock();
        }

        public async Task<Channel> GetChannel(string channelName)
        {
            Log($"Getting channel {channelName}");
            using (await door.LockAsync())
            {
                if (!channelsByName.ContainsKey(channelName))
                    channelsByName.Add(channelName, new Channel(channelName, logger));
                return channelsByName[channelName];
            }
        }

        void Log(string message) => logger.LogDebug($"{nameof(ChannelFactory)} - {message}");
    }
}
