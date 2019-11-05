namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitter
    {
        void TransmitSpecflowProjectCompilingEvent(IMsBuildTask task);
    }
}