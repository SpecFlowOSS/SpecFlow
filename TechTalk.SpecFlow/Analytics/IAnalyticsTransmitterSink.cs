using System.Threading.Tasks;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitterSink
    {
        Task<IResult> TransmitEvent(IAnalyticsEvent analyticsEvent);
    }
}
