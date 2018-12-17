using System;
using System.Threading.Tasks;

using NUnit.Framework;
using NUnit.Framework.Internal;

using PublishSubscribePattern;

namespace Tests.PublishSubscriberPattern {
    [TestFixture]
    public class Tests {
        [Test]
        public async Task PublishInSequence_SubscribersReceiveEvent ( ) {
            // Arrange
            int n = 0;
            void F ( string message ) {
                n++;
            }
            EventsBroker publisher = new EventsBroker ( );
            await publisher.SubscribeTo<string> ( F );
            await publisher.SubscribeTo<string> ( F );
            await publisher.SubscribeTo<string> ( F );
            await publisher.SubscribeTo<string> ( F );

            // Act
            await publisher.Publish ( "hey!" );

            // Assert
            Assert.AreEqual ( 4 , n );
        }
        [Test]
        public async Task PublishInParallel_SubscribersReceiveEvent ( ) {
            // Arrange
            int n = 0;
            object mutex = new object ( );
            void F ( string message ) {
                lock ( mutex ) {
                    n++;
                }
            }
            EventsBroker publisher = new EventsBroker ( );
            await publisher.SubscribeTo<string> ( F );
            await publisher.SubscribeTo<string> ( F );
            await publisher.SubscribeTo<string> ( F );
            await publisher.SubscribeTo<string> ( F );

            // Act
            await publisher.Publish ( "hey!" , asParallel: true );

            // Assert
            Assert.AreEqual ( 4 , n );
        }
        [Test]
        public async Task SubscribeTo_SubscriptionsStoreAction ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );

            // Act
            Guid id = await broker.SubscribeTo<string> ( null );

            // Assert
            Assert.IsTrue ( await broker.IsSubscribed ( id ) );
        }
        [Test]
        public async Task IsSubscribed_SubscriptionHasAction_ReturnsTrue ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );
            Guid id = Guid.NewGuid ( );

            // Act
            broker.Subscriptions.Add ( new Subscription ( null , id , typeof ( string ) ) );

            // Assert
            Assert.IsTrue ( await broker.IsSubscribed ( id ) );
        }
        [Test]
        public async Task IsSubscribed_SubscriptionHasNoAction_ReturnsFalse ( ) {
            // Arrange
            // Act
            DummyEventsBroker broker = new DummyEventsBroker ( );

            // Assert
            Assert.IsFalse ( await broker.IsSubscribed ( Guid.NewGuid ( ) ) );
        }
        [Test]
        public async Task Unsubscribe_SubscriptionHadAction_ActionRemoved ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );
            Guid id = await broker.SubscribeTo<string> ( null );
            Assert.IsTrue ( await broker.IsSubscribed ( id ) );

            // Act
            await broker.Unsubscribe ( id );

            // Assert
            Assert.IsFalse ( await broker.IsSubscribed ( id ) );
        }
        [Test]
        public async Task UnsubscribeFrom_SubscriptionHadManyActions_ActionsRemoved ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );
            Guid id1 = await broker.SubscribeTo<string> ( null );
            Guid id2 = await broker.SubscribeTo<string> ( null );
            Assert.IsTrue ( await broker.IsSubscribed ( id1 ) );
            Assert.IsTrue ( await broker.IsSubscribed ( id2 ) );

            // Act
            await broker.UnsubscribeFrom<string> ( );

            // Assert
            Assert.IsEmpty ( broker.Subscriptions );
        }
        [Test]
        public async Task ClearSubscriptions_SubscriptionHadManyActions_AllActionsRemoved ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );
            Guid id1 = await broker.SubscribeTo<string> ( null );
            Guid id2 = await broker.SubscribeTo<string> ( null );
            Assert.IsTrue ( await broker.IsSubscribed ( id1 ) );
            Assert.IsTrue ( await broker.IsSubscribed ( id2 ) );

            // Act
            await broker.ClearSubscriptions ( );

            // Assert
            Assert.IsEmpty ( broker.Subscriptions );
        }
    }
}
