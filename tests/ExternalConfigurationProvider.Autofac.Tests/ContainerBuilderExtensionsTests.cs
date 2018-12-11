using System;
using Autofac;
using ExternalConfiguration.Autofac;
using Moq;
using Xunit;

namespace ExternalConfigurationProvider.Autofac.Tests
{
    public class ContainerBuilderExtensionsTests
    {
        private Mock<ContainerBuilder> _containerBuilderMock = new Mock<ContainerBuilder>();

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public void RegisterConsulConfigurationProvider_ThrowsException_WhenEnvironmentIsNotSpecified(string environment)
        {
            var containerBuilder = _containerBuilderMock.Object;

            var exception = Assert.Throws<ArgumentNullException>(() => containerBuilder.RegisterConsulConfigurationProvider(environment, true, c => {}));

            Assert.Equal("environment", exception.ParamName);
        }

        [Theory]
        [InlineData("", typeof(ArgumentNullException))]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData("/consul", typeof(ArgumentException))]
        [InlineData("localhost", typeof(ArgumentException))]
        public void RegisterConsulConfigurationProvider_ThrowsArException_WhenConfigUrlIsBad(string url, Type exceptionType)
        {
            var containerBuilder = _containerBuilderMock.Object;

            var exception = Assert.Throws(exceptionType, () => containerBuilder.RegisterConsulConfigurationProvider(
                "dev", true,
                c => { c.Url = url; }));

            var argumentException = exception as ArgumentException;

            Assert.Equal("Url", argumentException?.ParamName);
        }
    }
}