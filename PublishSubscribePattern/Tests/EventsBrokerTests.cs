using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Nito.AsyncEx;
using Xunit;

namespace PublishSubscribePattern.Tests
{
    internal class EventsBrokerTests
    {
        //[Fact]
        //public async Task Publish_SynchronousSubscribersListenning_SubscribersReceiveEvent()
        //{
        //    // Arrange
        //    int n = 0;
        //    Task F(string message) { n++; return Task.CompletedTask; };
        //    Mock<ILogger<ChannelFactory>> logger = new Mock<ILogger<ChannelFactory>>();
        //    EventsBroker broker = new EventsBroker(new ChannelFactory(logger.Object));
        //    for (int i = 0; i < 10; i++)
        //        await broker.SubscribeTo<string>(F).ConfigureAwait(false);

        //    // Act
        //    await broker.Publish("hey!").ConfigureAwait(false);

        //    // Assert
        //    Assert.Equal(10, n);
        //}

        //[Fact]
        //public async Task Publish_AsynchronousSubscribersListenning_SubscribersReceiveEvent()
        //{
        //    // Arrange
        //    int n = 0;
        //    AsyncLock door = new AsyncLock();
        //    async Task F(string message)
        //    {
        //        using (await door.LockAsync())
        //        {
        //            await Task.Delay(10 * new Random().Next(0, 10)).ConfigureAwait(false);
        //            n++;
        //        }
        //    }

        //    Mock<ILogger> logger = new Mock<ILogger>();
        //    EventsBroker broker = new EventsBroker(new ChannelFactory(logger.Object));
        //    for (int i = 0; i < 10; i++)
        //        await broker.SubscribeTo<string>(F).ConfigureAwait(false);

        //    // Act
        //    await broker.Publish("hey!").ConfigureAwait(false);

        //    // Assert
        //    Assert.Equal(10, n);
        //}

        //[Fact]
        //public async Task UnsubscribeTo_SubscriptionPreviouslyAdded_SubscriptionRemoved()
        //{
        //    // Arrange
        //    Mock<ILogger> logger = new Mock<ILogger>();
        //    EventsBroker broker = new EventsBroker(new ChannelFactory(logger.Object));
        //    Guid id = await broker.SubscribeTo<string>(null).ConfigureAwait(false);

        //    // Act
        //    bool ok = await broker.Unsubscribe(id).ConfigureAwait(false);

        //    // Assert
        //    Assert.True(ok);
        //}

        //[Fact]
        //public async Task UnsubscribeTo_SubscriptionPreviouslyAdded_FalseReturned()
        //{
        //    // Arrange
        //    Mock<ILogger> logger = new Mock<ILogger>();
        //    EventsBroker broker = new EventsBroker(new ChannelFactory(logger.Object));

        //    // Act
        //    bool ok = await broker.Unsubscribe(Guid.NewGuid()).ConfigureAwait(false);

        //    // Assert
        //    Assert.False(ok);
        //}

        //[Fact]
        //public async Task PublishInFirstChannel_SeveralActiveChannels_OnlySubscriptionsInFirstChannelActivated()
        //{
        //    // Arrange
        //    Mock<ILogger> logger = new Mock<ILogger>();
        //    EventsBroker broker = new EventsBroker(new ChannelFactory(logger.Object));
        //    bool channel1Activated = false;
        //    bool channel2Activated = false;
        //    await broker.SubscribeTo<string>(str => { channel1Activated = true; return Task.CompletedTask; }, "channel 1").ConfigureAwait(false);
        //    await broker.SubscribeTo<string>(str => { channel2Activated = true; return Task.CompletedTask; }, "channel 2").ConfigureAwait(false);

        //    // Act
        //    await broker.Publish("Activate", "channel 1").ConfigureAwait(false);

        //    // Assert
        //    Assert.True(channel1Activated);
        //    Assert.False(channel2Activated);
        //}

        //[Fact]
        //public async Task PublishOneType_SeveralTypeSubscriptionsAdded_OnlySubscriptionOfPublishedTypeActivated()
        //{
        //    // Arrange 
        //    Mock<ILogger> logger = new Mock<ILogger>();
        //    EventsBroker broker = new EventsBroker(new ChannelFactory(logger.Object));
        //    bool integerTypeSubscriptionActivated = false;
        //    bool stringTypeSubscriptionActivated = false;
        //    await broker.SubscribeTo<string>(str => { stringTypeSubscriptionActivated = true; return Task.CompletedTask; }).ConfigureAwait(false);
        //    await broker.SubscribeTo<int>(str => { integerTypeSubscriptionActivated = true; return Task.CompletedTask; }).ConfigureAwait(false);

        //    // Act
        //    await broker.Publish(1).ConfigureAwait(false);

        //    // Assert
        //    Assert.True(integerTypeSubscriptionActivated);
        //    Assert.False(stringTypeSubscriptionActivated);
        //}
    }
}
