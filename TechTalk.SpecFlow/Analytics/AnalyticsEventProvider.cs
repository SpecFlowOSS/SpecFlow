using System;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Analytics.UserId;
using TechTalk.SpecFlow.UnitTestProvider;
using System.Runtime.InteropServices;

namespace TechTalk.SpecFlow.Analytics
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
            string userId = _userUniqueIdStore.GetUserId();
            string unitTestProvider = _unitTestProvider;
            string specFlowVersion = GetSpecFlowVersion();
            bool isBuildServer = IsBuildServerMode();
            bool isDockerContainer = IsRunningInDockerContainer();
            string hashedAssemblyName = ToSha256(assemblyName);
            string platform = GetOSPlatform();
            string platformDescription = RuntimeInformation.OSDescription;

            var compiledEvent = new SpecFlowProjectCompilingEvent(
                DateTime.UtcNow,
                userId,
                platform,
                platformDescription,
                specFlowVersion,
                unitTestProvider,
                isBuildServer,
                hashedAssemblyName,
                targetFrameworks,
                targetFramework,
                msbuildVersion,
                projectGuid,
                isDockerContainer);

            return compiledEvent;
        }

        public SpecFlowProjectRunningEvent CreateProjectRunningEvent(string testAssemblyName)
        {
            string userId = _userUniqueIdStore.GetUserId();
            string unitTestProvider = _unitTestProvider;
            string specFlowVersion = GetSpecFlowVersion();
            bool isBuildServer = IsBuildServerMode();
            string targetFramework = GetNetCoreVersion() ?? Environment.Version.ToString();
            bool isDockerContainer = IsRunningInDockerContainer();
            
            string hashedAssemblyName = ToSha256(testAssemblyName);
            string platform = GetOSPlatform();
            string platformDescription = RuntimeInformation.OSDescription;

            var runningEvent = new SpecFlowProjectRunningEvent(
                DateTime.UtcNow,
                userId,
                platform,
                platformDescription,
                specFlowVersion,
                unitTestProvider,
                isBuildServer,
                hashedAssemblyName,
                null,
                targetFramework,
                isDockerContainer);
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

            throw new InvalidOperationException("Platform cannot be identified");
        }

        private bool IsBuildServerMode()
        {
            bool isRunByTfs = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TF_BUILD"));
            bool isRunByTeamCity = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEAMCITY_VERSION"));

            return isRunByTfs || isRunByTeamCity;
        }

        private bool IsRunningInDockerContainer()
        {
            return !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER"));
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
            {
                return assemblyPath[netCoreAppIndex + 1];
            }

            return null;
        }
    }
}
