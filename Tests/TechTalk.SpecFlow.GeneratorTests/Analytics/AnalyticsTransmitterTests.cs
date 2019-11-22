using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.Analytics
{
    public class AnalyticsTransmitterTests
    {
        [Fact]
        public void TryTransmitEvent_AnalyticsDisabled_ShouldReturnSuccess()
        {
            // ARRANGE
            var analyticsTransmitterSinkMock = new Mock<IAnalyticsTransmitterSink>();
            var environmentSpecFlowTelemetryCheckerMock = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            environmentSpecFlowTelemetryCheckerMock.Setup(m => m.IsSpecFlowTelemetryEnabled())
                                                   .Returns(false);

            var analyticsEventMock = new Mock<IAnalyticsEvent>();
            var analyticsTransmitter = new AnalyticsTransmitter(analyticsTransmitterSinkMock.Object, environmentSpecFlowTelemetryCheckerMock.Object);

            // ACT
            var result = analyticsTransmitter.TransmitEvent(analyticsEventMock.Object);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess>();
        }

        [Fact]
        public void TryTransmitEvent_AnalyticsDisabled_ShouldNotCallSink()
        {
            // ARRANGE
            var analyticsTransmitterSinkMock = new Mock<IAnalyticsTransmitterSink>();
            var environmentSpecFlowTelemetryCheckerMock = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            environmentSpecFlowTelemetryCheckerMock.Setup(m => m.IsSpecFlowTelemetryEnabled())
                                                   .Returns(false);

            var analyticsEventMock = new Mock<IAnalyticsEvent>();
            var analyticsTransmitter = new AnalyticsTransmitter(analyticsTransmitterSinkMock.Object, environmentSpecFlowTelemetryCheckerMock.Object);

            // ACT
            analyticsTransmitter.TransmitEvent(analyticsEventMock.Object);

            // ASSERT
            analyticsTransmitterSinkMock.Verify(m => m.TransmitEvent(It.IsAny<IAnalyticsEvent>()), Times.Never);
        }

        [Fact]
        public void TransmitSpecFlowProjectCompilingEvent_AnalyticsEnabled_ShouldCallSink()
        {
            // ARRANGE
            var analyticsTransmitterSinkMock = new Mock<IAnalyticsTransmitterSink>();
            var environmentSpecFlowTelemetryCheckerMock = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            environmentSpecFlowTelemetryCheckerMock.Setup(m => m.IsSpecFlowTelemetryEnabled())
                                                   .Returns(true);

            var specFlowProjectCompilingEvent = It.IsAny<SpecFlowProjectCompilingEvent>();
            var analyticsTransmitter = new AnalyticsTransmitter(analyticsTransmitterSinkMock.Object, environmentSpecFlowTelemetryCheckerMock.Object);

            // ACT
            analyticsTransmitter.TransmitSpecFlowProjectCompilingEvent(specFlowProjectCompilingEvent);

            // ASSERT
            analyticsTransmitterSinkMock.Verify(m => m.TransmitEvent(It.IsAny<IAnalyticsEvent>()), Times.Once);
        }
    }

    public class AnalyticsEventProviderTests
    {

    }
}
