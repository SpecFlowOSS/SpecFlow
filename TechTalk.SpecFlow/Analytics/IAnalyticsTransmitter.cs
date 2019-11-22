using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitter
    {
        IResult TransmitSpecFlowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent);
        IResult TransmitSpecFlowProjectRunningEvent(SpecFlowProjectRunningEvent projectRunningEvent);
    }
}
