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
            try
            {
                if (!_environmentSpecFlowTelemetryChecker.IsSpecFlowTelemetryEnabled())
                {
                    return;
                }

                _analyticsTransmitterSink.TransmitEvent(projectCompilingEvent);
            }
            catch (Exception)
            {
                //nope
            }
        }
        
    }
}