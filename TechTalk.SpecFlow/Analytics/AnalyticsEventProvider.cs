using System;
using System.Reflection;
using System.Text;
using TechTalk.SpecFlow.Analytics.UserId;
using TechTalk.SpecFlow.UnitTestProvider;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using TechTalk.SpecFlow.EnvironmentAccess;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.Analytics
{
    public class AnalyticsEventProvider : IAnalyticsEventProvider
    {
        private readonly IUserUniqueIdStore _userUniqueIdStore;
        private readonly IEnvironmentWrapper _environmentWrapper;
        private readonly string _unitTestProvider;

        public AnalyticsEventProvider(IUserUniqueIdStore userUniqueIdStore, UnitTestProviderConfiguration unitTestProviderConfiguration, IEnvironmentWrapper environmentWrapper)
        {
            _userUniqueIdStore = userUniqueIdStore;
            _environmentWrapper = environmentWrapper;
            _unitTestProvider = unitTestProviderConfiguration.UnitTestProvider;
        }

        public SpecFlowProjectCompilingEvent CreateProjectCompilingEvent(string msbuildVersion, string assemblyName, string targetFrameworks, string targetFramework, string projectGuid)
        {
            string userId = _userUniqueIdStore.GetUserId();
            string unitTestProvider = _unitTestProvider;
            string specFlowVersion = GetSpecFlowVersion();
            string buildServerName = GetBuildServerName();
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
                buildServerName,
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
            string targetFramework = GetNetCoreVersion() ?? Environment.Version.ToString();
            bool isDockerContainer = IsRunningInDockerContainer();
            string buildServerName = GetBuildServerName();

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
                buildServerName,
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

        private readonly Dictionary<string, string> buildServerTypes
            = new Dictionary<string, string> {
                { "TF_BUILD","Azure Pipelines"},
                { "TEAMCITY_VERSION","TeamCity"},
                { "JENKINS_HOME","Jenkins"},
                { "GITHUB_ACTIONS","GitHub Actions"},
                { "GITLAB_CI","GitLab CI/CD"},
                { "CODEBUILD_BUILD_ID","AWS CodeBuild"},
                { "TRAVIS","Travis CI"},
                { "APPVEYOR","AppVeyor"},
                { "BITBUCKET_BUILD_NUMBER", "Bitbucket Pipelines" },
                { "bamboo_agentId", "Atlassian Bamboo" },
                { "CIRCLECI", "CircleCI" },
                { "GO_PIPELINE_NAME", "GoCD" },
                { "BUDDY", "Buddy" },
                { "NEVERCODE", "Nevercode" },
                { "SEMAPHORE", "SEMAPHORE" },
                { "BROWSERSTACK_USERNAME", "BrowserStack" },
                { "CF_BUILD_ID", "Codefresh" },
                { "Octopus.Project.Id", "Octopus Deploy" },

                { "CI_NAME", "CodeShip" }
            };

        private string GetBuildServerName()
        {
            foreach (var buildServerType in buildServerTypes)
            {
                var envVariable = _environmentWrapper.GetEnvironmentVariable(buildServerType.Key);
                if (envVariable is ISuccess<string>)
                    return buildServerType.Value;
            }
            return null;
        }

        private bool IsRunningInDockerContainer()
        {
            return _environmentWrapper.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") is ISuccess<string>;
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
            var assemblyPath = assembly.Location.Split(new[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            int netCoreAppIndex = Array.IndexOf(assemblyPath, "Microsoft.NETCore.App");
            if (netCoreAppIndex > 0 && netCoreAppIndex < assemblyPath.Length - 2)
            {
                return assemblyPath[netCoreAppIndex + 1];
            }

            return null;
        }
    }
}
