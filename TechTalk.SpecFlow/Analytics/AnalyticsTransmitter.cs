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

        public Task<IResult> TransmitSpecFlowProjectCompilingEventAsync(SpecFlowProjectCompilingEvent projectCompilingEvent)
        {
            return TransmitEventAsync(projectCompilingEvent);
        }

        public Task<IResult> TransmitSpecFlowProjectRunningEventAsync(SpecFlowProjectRunningEvent projectRunningEvent)
        {
            return TransmitEventAsync(projectRunningEvent);
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
