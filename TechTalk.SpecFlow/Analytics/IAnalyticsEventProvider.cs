namespace TechTalk.SpecFlow.Analytics
{
    public interface IAnalyticsEventProvider
    {
        SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(
            string msbuildVersion,
            string assemblyName,
            string targetFrameworks,
            string targetFramework,
            string projectGuid);

        SpecFlowProjectRunningEvent CreateProjectRunningEvent(string testAssemblyName);
    }
}
