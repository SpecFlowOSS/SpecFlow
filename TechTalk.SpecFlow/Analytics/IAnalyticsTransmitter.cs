using System.Threading.Tasks;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsTransmitter
    {
        Task<IResult> TransmitSpecFlowProjectCompilingEvent(SpecFlowProjectCompilingEvent projectCompilingEvent);
        Task<IResult> TransmitSpecFlowProjectRunningEvent(SpecFlowProjectRunningEvent projectRunningEvent);
    }
}
