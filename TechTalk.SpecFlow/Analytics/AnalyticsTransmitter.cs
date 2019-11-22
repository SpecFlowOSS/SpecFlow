using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public class AnalyticsTransmitter : IAnalyticsTransmitter
    {
        private readonly IAnalyticsTransmitterSink _analyticsTransmitterSink;
        private readonly IEnvironmentSpecFlowTelemetryChecker _environmentSpecFlowTelemetryChecker;

        public AnalyticsTransmitter(IAnalyticsTransmitterSink analyticsTransmitterSink, IEnvironmentSpecFlowTelemetryChecker environmentSpecFlowTelemetryChecker)
        {
            _analyticsTransmitterSink = analyticsTransmitterSink;
            _environmentSpecFlowTelemetryChecker = environmentSpecFlowTelemetryChecker;
        }

        public IResult TransmitSpecFlowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent)
        {
            return TransmitEvent(projectCompilingEvent);
        }

        public IResult TransmitSpecFlowProjectRunningEvent(SpecFlowProjectRunningEvent projectRunningEvent)
        {
            return TransmitEvent(projectRunningEvent);
        }

        public IResult TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            if (!_environmentSpecFlowTelemetryChecker.IsSpecFlowTelemetryEnabled())
            {
                return Result.Success();
            }

            return _analyticsTransmitterSink.TransmitEvent(analyticsEvent);
        }
    }
}
