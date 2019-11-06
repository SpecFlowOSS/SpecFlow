using TechTalk.SpecFlow.Analytics;

namespace SpecFlow.Tools.MsBuild.Generation.Analytics
{
    public interface IAnalyticsEventProvider
    {
        SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(string platform, string buildServerMode, 
            string msbuildVersion, string assemblyName, string targetFrameworks,
            string targetFrameworkMoniker, string projectGuid);
    }
}