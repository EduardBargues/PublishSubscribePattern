using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Nito.AsyncEx;
using Xunit;

namespace PublishSubscribePattern.Tests
{
    public class Tests
    {
        [Fact]
        public async Task Publish_SynchronousSubscribersListenning_SubscribersReceiveEvent()
        {
            // Arrange
            int n = 0;
            Task F(string message) { n++; return Task.CompletedTask; };
            EventsBroker publisher = new EventsBroker(logger: null);
            for (int i = 0; i < 10; i++)
                await publisher.SubscribeTo<string>(F);

            // Act
            await publisher.Publish("hey!");

            // Assert
            Assert.Equal(10, n);
        }

        [Fact]
        public async Task Publish_AsynchronousSubscribersListenning_SubscribersReceiveEvent()
        {
            // Arrange
            int n = 0;
            AsyncLock door = new AsyncLock();
            async Task F(string message)
            {
                using (await door.LockAsync())
                {
                    await Task.Delay(10 * new Random().Next(0, 10));
                    n++;
                }
            }

            EventsBroker publisher = new EventsBroker(logger: null);
            for (int i = 0; i < 10; i++)
                await publisher.SubscribeTo<string>(F);

            // Act
            await publisher.Publish("hey!");

            // Assert
            Assert.Equal(10, n);
        }

        [Fact]
        public async Task PublishAsPArallel_AsynchronousSubscribersListenning_SubscribersReceiveEvent()
        {
            // Arrange
            int n = 0;
            AsyncLock door = new AsyncLock();
            async Task F(string message)
            {
                using (await door.LockAsync())
                {
                    await Task.Delay(10);
                    n++;
                }
            }

            EventsBroker publisher = new EventsBroker(logger: null);
            for (int i = 0; i < 10; i++)
                await publisher.SubscribeTo<string>(F);


            // Act
            await publisher.Publish("hey!", asParallel: true);

            // Assert
            Assert.Equal(10, n);
        }

        [Fact]
        public async Task UnsubscribeTo_SubscriptionPreviouslyAdded_SubscriptionRemoved()
        {
            // Arrange
            EventsBroker broker = new EventsBroker(null);
            Guid id = await broker.SubscribeTo<string>(null);

            // Act
            bool ok = await broker.Unsubscribe(id);

            // Assert
            Assert.True(ok);
        }

        [Fact]
        public async Task UnsubscribeTo_SubscriptionPreviouslyAdded_FalseReturned()
        {
            // Arrange
            EventsBroker broker = new EventsBroker(null);

            // Act
            bool ok = await broker.Unsubscribe(Guid.NewGuid());

            // Assert
            Assert.False(ok);
        }

        [Fact]
        public async Task PublishInFirstChannel_SeveralActiveChannels_OnlySubscriptionsInFirstChannelActivated()
        {
            // Arrange
            EventsBroker broker = new EventsBroker(null);
            bool channel1Activated = false;
            bool channel2Activated = false;
            await broker.SubscribeTo<string>(str => { channel1Activated = true; return Task.CompletedTask; }, "channel 1");
            await broker.SubscribeTo<string>(str => { channel2Activated = true; return Task.CompletedTask; }, "channel 2");

            // Act
            await broker.Publish("Activate", "channel 1");

            // Assert
            Assert.True(channel1Activated);
            Assert.False(channel2Activated);
        }

        [Fact]
        public async Task PublishOneType_SeveralTypeSubscriptionsAdded_OnlySubscriptionOfPublishedTypeActivated()
        {
            // Arrange 
            EventsBroker broker = new EventsBroker(null);
            bool integerTypeSubscriptionActivated = false;
            bool stringTypeSubscriptionActivated = false;
            await broker.SubscribeTo<string>(str => { stringTypeSubscriptionActivated = true; return Task.CompletedTask; });
            await broker.SubscribeTo<int>(str => { integerTypeSubscriptionActivated = true; return Task.CompletedTask; });

            // Act
            await broker.Publish(1);

            // Assert
            Assert.True(integerTypeSubscriptionActivated);
            Assert.False(stringTypeSubscriptionActivated);
        }

        [Fact]
        public async Task Publish_AsPArallelVsStandardApproachComparison()
        {
            // Arrange
            static async Task F(string message) => await Task.Delay(1000);
            EventsBroker publisher = new EventsBroker(logger: null);
            for (int i = 0; i < 10; i++)
                await publisher.SubscribeTo<string>(F);

            // Act
            Stopwatch watch = new Stopwatch();
            watch.Start();
            await publisher.Publish("hey!", asParallel: true);
            watch.Stop();
            long millisecondsParallel = watch.ElapsedMilliseconds;
            watch.Reset();
            watch.Start();
            await publisher.Publish("hey!", asParallel: false);
            watch.Stop();
            long milliseconds = watch.ElapsedMilliseconds;

            // Assert
            Assert.True(millisecondsParallel < milliseconds);
        }
    }
}
