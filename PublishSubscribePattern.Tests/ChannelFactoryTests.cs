using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PublishSubscribePattern.Abstractions;
using Xunit;

namespace PublishSubscribePattern.Tests
{
    public class ChannelFactoryTests
    {
        [Fact]
        public void GetChannel_ChannelPreviouslyConfigured_ChannelProvided()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IOptions<ChannelFactoryConfiguration>> options = new Mock<IOptions<ChannelFactoryConfiguration>>();
            options.Setup(m => m.Value)
                .Returns(new ChannelFactoryConfiguration() { ChannelNames = new List<string>() { "c1" } });
            ChannelFactory factory = new ChannelFactory(logger.Object, options.Object);

            // Act
            IChannel channel = factory.GetChannel("c1");

            // Assert
            Assert.NotNull(channel);
        }

        [Fact]
        public void GetChannel_ChannelNotConfigured_ExceptionLaunched()
        {
            // Arrange
            Mock<ILogger> logger = new Mock<ILogger>();
            Mock<IOptions<ChannelFactoryConfiguration>> options = new Mock<IOptions<ChannelFactoryConfiguration>>();
            options.Setup(m => m.Value)
                .Returns(new ChannelFactoryConfiguration() { ChannelNames = new List<string>() });
            ChannelFactory factory = new ChannelFactory(logger.Object, options.Object);

            // Act
            // Assert
            Assert.Throws<KeyNotFoundException>(() => factory.GetChannel("c1"));
        }
    }
}
