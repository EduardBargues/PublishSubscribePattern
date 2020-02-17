using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace PublishSubscribePattern.Tests
{
    public class ChannelTests
    {
        class ChannelDummy : Channel
        {
            public ChannelDummy(string channelName, ILogger logger, IScheduler scheduler) : base(channelName, logger, scheduler)
            {
            }

            public Dictionary<Guid, Subscription> SubscriptionsById => subscriptionsById;
        }

        [Fact]
        public async Task Subscribe_SubscriptionChached()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IScheduler> scheduler = new Mock<IScheduler>();
            string channelName = "channel-name";
            Subscription subs = new Subscription(null, typeof(string));
            ChannelDummy channel = new ChannelDummy(channelName, logger.Object, scheduler.Object);

            // Act
            Guid id = await channel.Subscribe(subs).ConfigureAwait(false);

            // Assert
            Assert.Equal(subs, channel.SubscriptionsById[id]);
        }

        [Fact]
        public async Task Unsubscribe_ChannelContainsSubscription_SubscriptionRemoved()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IScheduler> scheduler = new Mock<IScheduler>();
            string channelName = "channel-name";
            Subscription subs = new Subscription(null, typeof(string));
            ChannelDummy channel = new ChannelDummy(channelName, logger.Object, scheduler.Object);
            Guid id = await channel.Subscribe(subs).ConfigureAwait(false);

            // Act
            bool ok = await channel.Unsubscribe(id).ConfigureAwait(false);

            // Assert
            Assert.True(ok);
            Assert.Empty(channel.SubscriptionsById);
        }

        [Fact]
        public async Task Unsubscribe_ChannelDoesNotContainSubscription_ReturnedFalse()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IScheduler> scheduler = new Mock<IScheduler>();
            string channelName = "channel-name";
            Subscription subs = new Subscription(null, typeof(string));
            ChannelDummy channel = new ChannelDummy(channelName, logger.Object, scheduler.Object);

            // Act
            bool ok = await channel.Unsubscribe(Guid.NewGuid()).ConfigureAwait(false);

            // Assert
            Assert.False(ok);
        }

        [Fact]
        public async Task Publish_SchedulerCalled()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IScheduler> scheduler = new Mock<IScheduler>();
            string channelName = "channel-name";
            Subscription subs = new Subscription(null, typeof(string));
            ChannelDummy channel = new ChannelDummy(channelName, logger.Object, scheduler.Object);

            // Act
            await channel.Publish("").ConfigureAwait(false);

            // Assert
            scheduler.Verify(m => m.Enqueue(It.IsAny<Func<Task>>()), Times.Once);
        }
    }
}
