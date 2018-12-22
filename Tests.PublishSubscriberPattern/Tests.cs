using System;

using NUnit.Framework;

using PublishSubscribePattern;

namespace Tests.PublishSubscriberPattern {
    [TestFixture]
    public class Tests {
        [Test]
        public void PublishInSequence_SubscribersReceiveEvent ( ) {
            // Arrange
            int n = 0;
            void F ( string message ) {
                n++;
            }
            EventsBroker publisher = new EventsBroker ( );
            publisher.SubscribeTo<string> ( F );
            publisher.SubscribeTo<string> ( F );
            publisher.SubscribeTo<string> ( F );
            publisher.SubscribeTo<string> ( F );

            // Act
            publisher.Publish ( "hey!" );

            // Assert
            Assert.AreEqual ( 4 , n );
        }
        [Test]
        public void PublishInParallel_SubscribersReceiveEvent ( ) {
            // Arrange
            int n = 0;
            object mutex = new object ( );
            void F ( string message ) {
                lock ( mutex ) {
                    n++;
                }
            }
            EventsBroker publisher = new EventsBroker ( );
            publisher.SubscribeTo<string> ( F );
            publisher.SubscribeTo<string> ( F );
            publisher.SubscribeTo<string> ( F );
            publisher.SubscribeTo<string> ( F );

            // Act
            publisher.Publish ( "hey!" , asParallel: true );

            // Assert
            Assert.AreEqual ( 4 , n );
        }
        [Test]
        public void SubscribeTo_SubscriptionsStoreAction ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );

            // Act
            Guid id = broker.SubscribeTo<string> ( null );

            // Assert
            Assert.IsTrue ( broker.IsSubscribed ( id ) );
        }
        [Test]
        public void IsSubscribed_SubscriptionHasAction_ReturnsTrue ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );
            Guid id = Guid.NewGuid ( );

            // Act
            broker.Subscriptions.TryAdd ( id , new Subscription ( null , id , typeof ( string ) ) );

            // Assert
            Assert.IsTrue ( broker.IsSubscribed ( id ) );
        }
        [Test]
        public void IsSubscribed_SubscriptionHasNoAction_ReturnsFalse ( ) {
            // Arrange
            // Act
            DummyEventsBroker broker = new DummyEventsBroker ( );

            // Assert
            Assert.IsFalse ( broker.IsSubscribed ( Guid.NewGuid ( ) ) );
        }
        [Test]
        public void Unsubscribe_SubscriptionHadAction_ActionRemoved ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );
            Guid id = broker.SubscribeTo<string> ( null );
            Assert.IsTrue ( broker.IsSubscribed ( id ) );

            // Act
            broker.Unsubscribe ( id );

            // Assert
            Assert.IsFalse ( broker.IsSubscribed ( id ) );
        }
        [Test]
        public void ClearSubscriptions_SubscriptionHadManyActions_AllActionsRemoved ( ) {
            // Arrange
            DummyEventsBroker broker = new DummyEventsBroker ( );
            Guid id1 = broker.SubscribeTo<string> ( null );
            Guid id2 = broker.SubscribeTo<string> ( null );
            Assert.IsTrue ( broker.IsSubscribed ( id1 ) );
            Assert.IsTrue ( broker.IsSubscribed ( id2 ) );

            // Act
            broker.ClearSubscriptions ( );

            // Assert
            Assert.IsEmpty ( broker.Subscriptions );
        }
    }
}
