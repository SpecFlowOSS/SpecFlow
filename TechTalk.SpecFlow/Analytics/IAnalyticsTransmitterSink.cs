using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        IResult TransmitEvent(IAnalyticsEvent analyticsEvent);
    }
}
