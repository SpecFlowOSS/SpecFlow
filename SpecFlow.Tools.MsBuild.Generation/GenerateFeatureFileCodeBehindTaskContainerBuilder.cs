using BoDi;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Analytics;
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
            objectContainer.RegisterTypeAs<AnalyticsEventProvider, IAnalyticsEventProvider>();
            objectContainer.RegisterTypeAs<MSBuildTaskAnalyticsTransmitter, IMSBuildTaskAnalyticsTransmitter>();
            objectContainer.RegisterTypeAs<ExceptionTaskLogger, IExceptionTaskLogger>();

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
