using System;
using System.Runtime.InteropServices;
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

        public SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(string msbuildVersion, string assemblyName, string targetFrameworks, string targetFrameworkMoniker, string projectGuid)
        {
            var userId = _userUniqueIdStore.GetUserId();

            var unittestProvider = _unitTestProvider;
            var specFlowVersion = GetSpecFlowVersion();
            var isBuildServer = IsBuildServerMode();
            var hashedAssemblyName = ToSha256(assemblyName);

            var compiledEvent = new SpecFlowProjectCompilingEvent(DateTime.UtcNow, userId,
                platform, specFlowVersion, unittestProvider, isBuildServer,
                hashedAssemblyName, targetFrameworks, targetFrameworkMoniker, msbuildVersion, 
                projectGuid);
            return compiledEvent;
        }

        private bool IsBuildServerMode()
        {
            var isRunByTfs = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TF_BUILD"));
            var isRunByTeamCity = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));

            return isRunByTfs || isRunByTeamCity;
        }

        private string GetSpecFlowVersion()
        {
            return ThisAssembly.AssemblyInformationalVersion;
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
