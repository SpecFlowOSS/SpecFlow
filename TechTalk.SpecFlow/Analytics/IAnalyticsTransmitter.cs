using System.Threading.Tasks;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitter
    {
        bool IsEnabled { get; }

        Task<IResult> TransmitSpecFlowProjectCompilingEventAsync(SpecFlowProjectCompilingEvent projectCompilingEvent);
        Task<IResult> TransmitSpecFlowProjectRunningEventAsync(SpecFlowProjectRunningEvent projectRunningEvent);
    }
}
