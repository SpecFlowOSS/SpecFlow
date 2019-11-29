using System.Threading.Tasks;
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

        public async Task<IResult> TransmitSpecFlowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent)
        {
            return await TransmitEvent(projectCompilingEvent);
        }

        public async Task<IResult> TransmitSpecFlowProjectRunningEvent(SpecFlowProjectRunningEvent projectRunningEvent)
        {
            return await TransmitEvent(projectRunningEvent);
        }

        public async Task<IResult> TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            if (!_environmentSpecFlowTelemetryChecker.IsSpecFlowTelemetryEnabled())
            {
                return Result.Success();
            }

            return await _analyticsTransmitterSink.TransmitEvent(analyticsEvent);
        }
    }
}
