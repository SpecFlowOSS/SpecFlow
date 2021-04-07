using System.Threading.Tasks;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public class AnalyticsTransmitter : IAnalyticsTransmitter
    {
        private readonly IAnalyticsTransmitterSink _analyticsTransmitterSink;
        private readonly IEnvironmentSpecFlowTelemetryChecker _environmentSpecFlowTelemetryChecker;

        public bool IsEnabled => _environmentSpecFlowTelemetryChecker.IsSpecFlowTelemetryEnabled();

        public AnalyticsTransmitter(IAnalyticsTransmitterSink analyticsTransmitterSink, IEnvironmentSpecFlowTelemetryChecker environmentSpecFlowTelemetryChecker)
        {
            _analyticsTransmitterSink = analyticsTransmitterSink;
            _environmentSpecFlowTelemetryChecker = environmentSpecFlowTelemetryChecker;
        }

        public Task<IResult> TransmitSpecFlowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent)
        {
            return TransmitEvent(projectCompilingEvent);
        }

        public Task<IResult> TransmitSpecFlowProjectRunningEvent(SpecFlowProjectRunningEvent projectRunningEvent)
        {
            return TransmitEvent(projectRunningEvent);
        }

        public Task<IResult> TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            if (!IsEnabled)
            {
                return Task.FromResult(Result.Success());
            }

            return _analyticsTransmitterSink.TransmitEvent(analyticsEvent);
        }
    }
}
