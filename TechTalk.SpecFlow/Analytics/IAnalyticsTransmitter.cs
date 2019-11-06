namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitter
    {
        void TransmitSpecflowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent);
    }
}