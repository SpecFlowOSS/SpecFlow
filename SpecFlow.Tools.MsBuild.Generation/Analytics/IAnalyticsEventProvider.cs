using TechTalk.SpecFlow.Analytics;

namespace SpecFlow.Tools.MsBuild.Generation.Analytics
{
    public interface IAnalyticsEventProvider
    {
        SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(string msbuildVersion, 
            string assemblyName, string targetFrameworks, string targetFramework, string projectGuid);
    }
}