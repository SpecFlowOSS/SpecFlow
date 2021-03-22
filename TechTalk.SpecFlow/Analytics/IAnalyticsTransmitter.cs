using System.Threading.Tasks;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitter
    {
        bool IsEnabled { get; }

        Task<IResult> TransmitSpecFlowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent);
        Task<IResult> TransmitSpecFlowProjectRunningEvent(SpecFlowProjectRunningEvent projectRunningEvent);
    }
}
