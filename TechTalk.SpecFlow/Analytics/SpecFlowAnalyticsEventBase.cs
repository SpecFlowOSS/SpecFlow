using System;

namespace TechTalk.SpecFlow.Analytics
{
    public abstract class SpecFlowAnalyticsEventBase : IAnalyticsEvent
    {
        public abstract string EventName { get; }
        public DateTime UtcDate { get; }
        public string UserId { get; }
        public string Platform { get; }
        public string PlatformDescription { get; }
        public string SpecFlowVersion { get; }
        public string UnitTestProvider { get; }
        public bool IsBuildServer { get; }
        public string BuildServerName { get; }
        public string HashedAssemblyName { get; }
        public string TargetFrameworks { get; }
        public string TargetFramework { get; }
        public bool IsDockerContainer { get; }

        protected SpecFlowAnalyticsEventBase(DateTime utcDate, string userId, string platform, string platformDescription, string specFlowVersion, string unitTestProvider, string buildServerName, string hashedAssemblyName, string targetFrameworks, string targetFramework, bool isDockerContainer)
        {
            UtcDate = utcDate;
            UserId = userId;
            Platform = platform;
            PlatformDescription = platformDescription;
            SpecFlowVersion = specFlowVersion;
            UnitTestProvider = unitTestProvider;
            BuildServerName = buildServerName;
            IsBuildServer = !string.IsNullOrWhiteSpace(buildServerName);
            HashedAssemblyName = hashedAssemblyName;
            TargetFrameworks = targetFrameworks;
            TargetFramework = targetFramework;
            IsDockerContainer = isDockerContainer;
        }
    }
}
