using System;
using System.Threading.Tasks;
using Moq;
using PublishSubscribePattern.Abstractions;
using Xunit;

namespace PublishSubscribePattern.Tests
{
    public class EventsBrokerTests
    {
        [Fact]
        public async Task Subscribe_ChannelFactoryProvidesChannelAndChannelSubscribes()
        {
            // Arrange 
            string channelName = "channel-name";
            Mock<IChannelFactory> mockChannelFactory = new Mock<IChannelFactory>();
            Mock<IChannel> mockChannel = ChannelFactoryProvidesChannel(mockChannelFactory, channelName);
            Subscription subscription = null;
            mockChannel
                .Setup(m => m.Subscribe(It.IsAny<Subscription>()))
                .Callback<Subscription>(sub => subscription = sub);
            EventsBroker eb = new EventsBroker(mockChannelFactory.Object);

            // Act 
            Guid id = await eb.Subscribe<string>(str => Task.CompletedTask, channelName)
                .ConfigureAwait(false);

            // Assert
            mockChannelFactory.Verify(m => m.GetChannel(channelName), Times.Once);
            mockChannel.Verify(m => m.Subscribe(It.IsAny<Subscription>()), Times.Once);
            Assert.NotNull(subscription);
            Assert.Equal(typeof(string), subscription.EventType);
        }

        [Fact]
        public async Task Unsubscribe_ChannelFactoryProvidesChannelAndChannelUnsubscribes()
        {
            // Arrange 
            string channelName = "channel-name";
            Mock<IChannelFactory> mockChannelFactory = new Mock<IChannelFactory>();
            Mock<IChannel> mockChannel = ChannelFactoryProvidesChannel(mockChannelFactory, channelName);
            Guid unsubscribedId = Guid.Empty;
            mockChannel
                .Setup(m => m.Unsubscribe(It.IsAny<Guid>()))
                .Callback<Guid>(id => unsubscribedId = id);
            EventsBroker eb = new EventsBroker(mockChannelFactory.Object);
            Guid requestId = Guid.NewGuid();

            // Act 
            await eb.Unsubscribe(requestId, channelName).ConfigureAwait(false);

            // Assert
            mockChannelFactory.Verify(m => m.GetChannel(channelName), Times.Once);
            mockChannel.Verify(m => m.Unsubscribe(It.IsAny<Guid>()), Times.Once);
            Assert.NotEqual(Guid.Empty, unsubscribedId);
            Assert.Equal(requestId, unsubscribedId);
        }

        [Fact]
        public async Task Publish_ChannelFactoryProvidesChannelAndChannelPublishes()
        {
            // Arrange 
            string channelName = "channel-name";
            Mock<IChannelFactory> mockChannelFactory = new Mock<IChannelFactory>();
            Mock<IChannel> mockChannel = ChannelFactoryProvidesChannel(mockChannelFactory, channelName);
            string publishedString = null;
            mockChannel
                .Setup(m => m.Publish(It.IsAny<string>()))
                .Callback<string>(str => publishedString = str);
            string requestString = "hey!";
            EventsBroker eb = new EventsBroker(mockChannelFactory.Object);

            // Act 
            await eb.Publish(requestString, channelName).ConfigureAwait(false);

            // Assert
            mockChannelFactory.Verify(m => m.GetChannel(channelName), Times.Once);
            mockChannel.Verify(m => m.Publish(It.IsAny<string>()), Times.Once);
            Assert.Equal(requestString, publishedString);
        }

        private Mock<IChannel> ChannelFactoryProvidesChannel(Mock<IChannelFactory> mock, string channelName)
        {
            Mock<IChannel> mockChannel = new Mock<IChannel>();
            mock
                .Setup(m => m.GetChannel(channelName))
                .Returns(mockChannel.Object);

            return mockChannel;
        }
    }
}