using System;

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

        public void TransmitSpecflowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent)
        {
            TransmitEvent(projectCompilingEvent);
        }

        public void TransmitSpecflowProjectRunningEvent(SpecFlowProjectRunningEvent projectRunningEvent)
        {
            TransmitEvent(projectRunningEvent);
        }

        private void TransmitEvent(IAnalyticsEvent analyticsEvent)
        {
            try
            {
                if (!_environmentSpecFlowTelemetryChecker.IsSpecFlowTelemetryEnabled())
                {
                    return;
                }

                _analyticsTransmitterSink.TransmitEvent(analyticsEvent);
            }
            catch (Exception)
            {
                //nope
            }
        }
    }
}