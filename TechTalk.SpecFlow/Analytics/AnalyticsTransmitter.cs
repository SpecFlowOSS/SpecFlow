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

        public async Task<IResult> TransmitSpecFlowProjectCompilingEventAsync(SpecFlowProjectCompilingEvent projectCompilingEvent)
        {
            return await TransmitEventAsync(projectCompilingEvent);
        }

        public async Task<IResult> TransmitSpecFlowProjectRunningEventAsync(SpecFlowProjectRunningEvent projectRunningEvent)
        {
            return await TransmitEventAsync(projectRunningEvent);
        }

        public async Task<IResult> TransmitEventAsync(IAnalyticsEvent analyticsEvent)
        {
            if (!IsEnabled)
            {
                return Result.Success();
            }

            return await _analyticsTransmitterSink.TransmitEventAsync(analyticsEvent);
        }
    }
}
