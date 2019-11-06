using System;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Analytics.UserId;
using TechTalk.SpecFlow.UnitTestProvider;

namespace SpecFlow.Tools.MsBuild.Generation.Analytics
{
    public class AnalyticsEventProvider : IAnalyticsEventProvider
    {
        private readonly IUserUniqueIdStore _userUniqueIdStore;
        private readonly string _unitTestProvider;

        public AnalyticsEventProvider(IUserUniqueIdStore userUniqueIdStore, UnitTestProviderConfiguration unitTestProviderConfiguration)
        {
            _userUniqueIdStore = userUniqueIdStore;
            _unitTestProvider = unitTestProviderConfiguration.UnitTestProvider;
        }

        public SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(string platform, string buildServerMode, string msbuildVersion, string assemblyName, string targetFrameworks, string targetFrameworkMoniker, string projectGuid)
        {
            var userId = _userUniqueIdStore.GetUserId();

            var unittestProvider = _unitTestProvider;
            var specFlowVersion = GetSpecFlowVersion();
            var isBuildServer = GetIsBuildServerMode(buildServerMode);
            var hashedAssemblyName = ToSha256(assemblyName);

            var compiledEvent = new SpecFlowProjectCompilingEvent(DateTime.UtcNow, userId,
                platform, specFlowVersion, unittestProvider, isBuildServer,
                hashedAssemblyName, targetFrameworks, targetFrameworkMoniker, msbuildVersion, 
                projectGuid);
            return compiledEvent;
        }

        private string GetSpecFlowVersion()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            return version.ToString(2);
        }

        private bool GetIsBuildServerMode(string buildServerMode)
        {
            if (string.IsNullOrEmpty(buildServerMode))
            {
                return false;
            }
            return bool.Parse(buildServerMode);
        }

        private string ToSha256(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return string.Empty;
            }
            var crypt = new System.Security.Cryptography.SHA256Managed();
            var stringBuilder = new StringBuilder();
            var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(inputString));
            foreach (byte theByte in crypto)
            {
                stringBuilder.Append(theByte.ToString("x2"));
            }
            return stringBuilder.ToString();
        }
    }
}
