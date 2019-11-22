using TechTalk.SpecFlow.CommonModels;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IMSBuildTaskAnalyticsTransmitter
    {
        IResult TryTransmitProjectCompilingEvent();
    }
}
