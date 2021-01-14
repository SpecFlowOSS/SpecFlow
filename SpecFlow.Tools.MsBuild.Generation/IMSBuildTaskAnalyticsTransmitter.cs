using System.Threading.Tasks;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IMSBuildTaskAnalyticsTransmitter
    {
        Task TryTransmitProjectCompilingEventAsync();
    }
}
