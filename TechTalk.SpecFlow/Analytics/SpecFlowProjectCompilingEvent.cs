using System;

namespace TechTalk.SpecFlow.Analytics
{
    public class SpecFlowProjectCompilingEvent : SpecFlowAnalyticsEventBase
    {
        public string MSBuildVersion { get; }
        public string ProjectGuid { get; set; }

        public SpecFlowProjectCompilingEvent(DateTime utcDate, string userId, string platform, string platformDescription, string specFlowVersion, string unitTestProvider, string buildServerName, string hashedAssemblyName, string targetFrameworks, string targetFramework, string msBuildVersion, string projectGuid, bool isDockerContainer) : base(utcDate, userId, platform, platformDescription, specFlowVersion, unitTestProvider, buildServerName, hashedAssemblyName, targetFrameworks, targetFramework, isDockerContainer)
        {
            MSBuildVersion = msBuildVersion;
            ProjectGuid = projectGuid;
        }

        public override string EventName => "Compiling SpecFlow project";
    }
}