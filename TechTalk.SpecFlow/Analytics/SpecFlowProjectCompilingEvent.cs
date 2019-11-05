using System;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Analytics
{
    public class SpecFlowProjectCompilingEvent : SpecFlowAnalyticsEventBase
    {
        public string MSBuildVersion { get; }
        public string ProjectGuid { get; set; }

        public SpecFlowProjectCompilingEvent(DateTime utcDate, string userId, string platform, string specFlowVersion, string unitTestProvider, bool isBuildServer, string hashedAssemblyName, string targetFrameworks, string targetFrameworkMoniker, string msBuildVersion, string projectGuid) : base(utcDate, userId, platform, specFlowVersion, unitTestProvider, isBuildServer, hashedAssemblyName, targetFrameworks, targetFrameworkMoniker)
        {
            MSBuildVersion = msBuildVersion;
            ProjectGuid = projectGuid;
        }

        public override string EventName => "Compiling SpecFlow project";
    }
}