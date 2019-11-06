using Moq;
using TechTalk.SpecFlow.Analytics;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.MSBuildTask
{
    public class AnalyticsTransmitterTests
    {
        Mock<IEnvironmentSpecFlowTelemetryChecker> environmentTelemetryChecker;
        Mock<IAnalyticsTransmitterSink> analyticsTransmitterSink;
        AnalyticsTransmitter sut;

        public AnalyticsTransmitterTests()
        {
            environmentTelemetryChecker = new Mock<IEnvironmentSpecFlowTelemetryChecker>();
            analyticsTransmitterSink = new Mock<IAnalyticsTransmitterSink>();
            
            sut = new AnalyticsTransmitter(analyticsTransmitterSink.Object, environmentTelemetryChecker.Object);
        }

        private void GivenAnalyticsEnabled()
        {
            environmentTelemetryChecker.Setup(analyticsChecker => analyticsChecker.IsSpecFlowTelemetryEnabled()).Returns(true);
        }

        private void GivenAnalyticsDisabled()
        {
            environmentTelemetryChecker.Setup(analyticsChecker => analyticsChecker.IsSpecFlowTelemetryEnabled()).Returns(false);
        }

        [Fact]
        public void Should_NotSendAnalytics_WhenDisabled()
        {
            GivenAnalyticsDisabled();

            sut.TransmitSpecflowProjectCompilingEvent(It.IsAny<SpecFlowProjectCompilingEvent>());

            environmentTelemetryChecker.Verify(telemetryChecker => telemetryChecker.IsSpecFlowTelemetryEnabled(), Times.Once);
            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.IsAny<IAnalyticsEvent>()), Times.Never);
        }

        [Fact]
        public void Should_SendProjectCompilingEvent_WhenAnalyticsEnabled()
        {
            GivenAnalyticsEnabled();

            sut.TransmitSpecflowProjectCompilingEvent(It.IsAny<SpecFlowProjectCompilingEvent>());

            environmentTelemetryChecker.Verify(telemetryChecker => telemetryChecker.IsSpecFlowTelemetryEnabled(), Times.Once);
            analyticsTransmitterSink.Verify(sink => sink.TransmitEvent(It.IsAny<IAnalyticsEvent>()), Times.Once);
        }
    }
}
