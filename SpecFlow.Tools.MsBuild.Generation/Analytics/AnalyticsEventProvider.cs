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

        public SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(string msbuildVersion, string assemblyName, string targetFrameworks, string targetFramework, string projectGuid)
        {
            var userId = _userUniqueIdStore.GetUserId();
            var unittestProvider = _unitTestProvider;
            var specFlowVersion = GetSpecFlowVersion();
            var isBuildServer = IsBuildServerMode();
            var hashedAssemblyName = ToSha256(assemblyName);
            var platform = GetOSPlatform();
            var platformDescription = RuntimeInformation.OSDescription;

            var compiledEvent = new SpecFlowProjectCompilingEvent(DateTime.UtcNow, userId,
                platform, platformDescription, specFlowVersion, unittestProvider, isBuildServer,
                hashedAssemblyName, targetFrameworks, targetFramework, msbuildVersion,
                projectGuid);
            return compiledEvent;
        }

        private string GetOSPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "Windows";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "Linux";
            }
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "OSX";
            }

            return "Platform cannot be identified";
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

        private string ToSha256(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
            {
                return null;
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
