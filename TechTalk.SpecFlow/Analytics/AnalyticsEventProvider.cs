using System;
using System.Linq;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Analytics.UserId;
using TechTalk.SpecFlow.UnitTestProvider;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace TechTalk.SpecFlow.Analytics
{
    public class AnalyticsEventProvider : IAnalyticsEventProvider
    {
        private readonly IUserUniqueIdStore _userUniqueIdStore;
        private readonly ITestRunnerManager _testRunnerManager;
        private readonly string _unitTestProvider;

        public AnalyticsEventProvider(IUserUniqueIdStore userUniqueIdStore, UnitTestProviderConfiguration unitTestProviderConfiguration, ITestRunnerManager testRunnerManager)
        {
            _userUniqueIdStore = userUniqueIdStore;
            _testRunnerManager = testRunnerManager;
            _unitTestProvider = unitTestProviderConfiguration.UnitTestProvider;
        }

        public SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(string msbuildVersion, string assemblyName, string targetFrameworks, string targetFramework, string projectGuid)
        {
            var userId = _userUniqueIdStore.GetUserId();
            var unitTestProvider = _unitTestProvider;
            var specFlowVersion = GetSpecFlowVersion();
            var isBuildServer = IsBuildServerMode();
            var hashedAssemblyName = ToSha256(assemblyName);
            var platform = GetOSPlatform();
            var platformDescription = RuntimeInformation.OSDescription;

            var compiledEvent = new SpecFlowProjectCompilingEvent(DateTime.UtcNow, userId,
                platform, platformDescription, specFlowVersion, unitTestProvider, isBuildServer,
                hashedAssemblyName, targetFrameworks, targetFramework, msbuildVersion,
                projectGuid);
            return compiledEvent;
        }

        public SpecFlowProjectRunningEvent CreateProjectRunningEvent()
        {
            var userId = _userUniqueIdStore.GetUserId();
            var unitTestProvider = _unitTestProvider;
            var specFlowVersion = GetSpecFlowVersion();
            var isBuildServer = IsBuildServerMode();
            var assembly = _testRunnerManager.TestAssembly;
            var assemblyName = assembly.GetName().Name;
            var targetFramework = GetNetCoreVersion() ?? Environment.Version.ToString();
            
            var hashedAssemblyName = ToSha256(assemblyName);
            var platform = GetOSPlatform();
            var platformDescription = RuntimeInformation.OSDescription;

            var runningEvent = new SpecFlowProjectRunningEvent(DateTime.UtcNow, userId,
                platform, platformDescription, specFlowVersion, unitTestProvider, isBuildServer,
                hashedAssemblyName, null, targetFramework);
            return runningEvent;
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

        private string GetNetCoreVersion()
        {
            var assembly = typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly;
            var assemblyPath = assembly.CodeBase.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
                return assemblyPath[netCoreAppIndex + 1];
            return null;
        }
    }
}
