using System.Runtime.CompilerServices;
using System.Threading.Tasks;
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace PublishSubscribePattern
{
    internal interface IChannelFactory
    {
        IChannel GetChannel(string channelName);
    }
}