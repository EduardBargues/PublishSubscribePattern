using System.Threading.Tasks;

namespace PublishSubscribePattern
{
    internal interface IChannelFactory
    {
        Task<Channel> GetChannel(string channelName);
    }
}