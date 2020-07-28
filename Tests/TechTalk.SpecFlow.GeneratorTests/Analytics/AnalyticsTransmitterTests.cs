using System.Threading.Tasks;
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
        public async Task TryTransmitEvent_AnalyticsDisabled_ShouldReturnSuccess()
        {
            // ARRANGE
            var analyticsTransmitterSinkMock = new Mock<IAnalyticsTransmitterSink>();
            var environmentSpecFlowTelemetryCheckerMock = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            environmentSpecFlowTelemetryCheckerMock.Setup(m => m.IsSpecFlowTelemetryEnabled())
                                                   .Returns(false);

            var analyticsEventMock = new Mock<IAnalyticsEvent>();
            var analyticsTransmitter = new AnalyticsTransmitter(analyticsTransmitterSinkMock.Object, environmentSpecFlowTelemetryCheckerMock.Object);

            // ACT
            var result = await analyticsTransmitter.TransmitEvent(analyticsEventMock.Object);

            // ASSERT
            result.Should().BeAssignableTo<ISuccess>();
        }

        [Fact]
        public async Task TryTransmitEvent_AnalyticsDisabled_ShouldNotCallSink()
        {
            // ARRANGE
            var analyticsTransmitterSinkMock = new Mock<IAnalyticsTransmitterSink>();
            var environmentSpecFlowTelemetryCheckerMock = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            environmentSpecFlowTelemetryCheckerMock.Setup(m => m.IsSpecFlowTelemetryEnabled())
                                                   .Returns(false);

            var analyticsEventMock = new Mock<IAnalyticsEvent>();
            var analyticsTransmitter = new AnalyticsTransmitter(analyticsTransmitterSinkMock.Object, environmentSpecFlowTelemetryCheckerMock.Object);

            // ACT
            await analyticsTransmitter.TransmitEvent(analyticsEventMock.Object);

            // ASSERT
            analyticsTransmitterSinkMock.Verify(sink => sink.TransmitEvent(It.IsAny<IAnalyticsEvent>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task TransmitSpecFlowProjectCompilingEvent_AnalyticsEnabled_ShouldCallSink()
        {
            // ARRANGE
            var analyticsTransmitterSinkMock = new Mock<IAnalyticsTransmitterSink>();
            var environmentSpecFlowTelemetryCheckerMock = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            environmentSpecFlowTelemetryCheckerMock.Setup(m => m.IsSpecFlowTelemetryEnabled())
                                                   .Returns(true);

            var specFlowProjectCompilingEvent = It.IsAny<SpecFlowProjectCompilingEvent>();
            var analyticsTransmitter = new AnalyticsTransmitter(analyticsTransmitterSinkMock.Object, environmentSpecFlowTelemetryCheckerMock.Object);

            // ACT
            await analyticsTransmitter.TransmitSpecFlowProjectCompilingEvent(specFlowProjectCompilingEvent);

            // ASSERT
            analyticsTransmitterSinkMock.Verify(m => m.TransmitEvent(It.IsAny<IAnalyticsEvent>(), It.IsAny<string>()), Times.Once);
        }
    }
}
