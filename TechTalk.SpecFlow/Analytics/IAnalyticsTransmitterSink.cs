using System.Threading.Tasks;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        Task TransmitEvent(IAnalyticsEvent analyticsEvent);
    }
}
