using System.Threading.Tasks;
using TechTalk.SpecFlow.Analytics.AppInsights;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        Task<IResult> TransmitEvent(IAnalyticsEvent analyticsEvent, string instrumentationKey = AppInsightsInstrumentationKey.Key);
    }
}
