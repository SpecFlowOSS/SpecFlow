using BoDi;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Analytics.AppInsights;
using TechTalk.SpecFlow.Analytics.UserId;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.EnvironmentAccess;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Project;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTaskContainerBuilder
    {
        public IObjectContainer BuildRootContainer(
            TaskLoggingHelper taskLoggingHelper,
            SpecFlowProjectInfo specFlowProjectInfo,
            IMSBuildInformationProvider msbuildInformationProvider,
            GenerateFeatureFileCodeBehindTaskConfiguration generateFeatureFileCodeBehindTaskConfiguration)
        {
            var objectContainer = new ObjectContainer();

            // singletons
            objectContainer.RegisterInstanceAs(taskLoggingHelper);
            objectContainer.RegisterInstanceAs(specFlowProjectInfo);
            objectContainer.RegisterInstanceAs(msbuildInformationProvider);
            objectContainer.RegisterInstanceAs(generateFeatureFileCodeBehindTaskConfiguration);

            // types
            objectContainer.RegisterTypeAs<TaskLoggingHelperWithNameTagWrapper, ITaskLoggingWrapper>();
            objectContainer.RegisterTypeAs<SpecFlowProjectProvider, ISpecFlowProjectProvider>();
            objectContainer.RegisterTypeAs<MSBuildProjectReader, IMSBuildProjectReader>();
            objectContainer.RegisterTypeAs<ProcessInfoDumper, IProcessInfoDumper>();
            objectContainer.RegisterTypeAs<AssemblyResolveLoggerFactory, IAssemblyResolveLoggerFactory>();
            objectContainer.RegisterTypeAs<GenerateFeatureFileCodeBehindTaskExecutor, IGenerateFeatureFileCodeBehindTaskExecutor>();
            objectContainer.RegisterTypeAs<MSBuildTaskAnalyticsTransmitter, IMSBuildTaskAnalyticsTransmitter>();
            objectContainer.RegisterTypeAs<ExceptionTaskLogger, IExceptionTaskLogger>();

            objectContainer.RegisterTypeAs<FileUserIdStore, IUserUniqueIdStore>();
            objectContainer.RegisterTypeAs<FileService, IFileService>();
            objectContainer.RegisterTypeAs<DirectoryService, IDirectoryService>();
            objectContainer.RegisterTypeAs<EnvironmentWrapper, IEnvironmentWrapper>();

            objectContainer.RegisterTypeAs<EnvironmentSpecFlowTelemetryChecker, IEnvironmentSpecFlowTelemetryChecker>();
            objectContainer.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            objectContainer.RegisterTypeAs<HttpClientAnalyticsTransmitterSink, IAnalyticsTransmitterSink>();
            objectContainer.RegisterTypeAs<AppInsightsEventSerializer, IAppInsightsEventSerializer>();
            objectContainer.RegisterTypeAs<HttpClientWrapper, HttpClientWrapper>();
            objectContainer.RegisterTypeAs<AnalyticsEventProvider, IAnalyticsEventProvider>();
            objectContainer.RegisterTypeAs<ConfigurationLoader, IConfigurationLoader>();
            objectContainer.RegisterTypeAs<GeneratorConfigurationProvider, IGeneratorConfigurationProvider>();
            objectContainer.RegisterTypeAs<ProjectReader, ISpecFlowProjectReader>();
            objectContainer.RegisterTypeAs<SpecFlowJsonLocator, ISpecFlowJsonLocator>();

            if (generateFeatureFileCodeBehindTaskConfiguration.OverrideAnalyticsTransmitter is null)
            {
                objectContainer.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            }
            else
            {
                objectContainer.RegisterInstanceAs(generateFeatureFileCodeBehindTaskConfiguration.OverrideAnalyticsTransmitter);
            }

            return objectContainer;
        }
    }
}
