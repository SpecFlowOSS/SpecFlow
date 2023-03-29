using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Tracing;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class WrappedGeneratorContainerBuilder
    {
        private readonly GeneratorContainerBuilder _generatorContainerBuilder;
        private readonly GenerateFeatureFileCodeBehindTaskConfiguration _generateFeatureFileCodeBehindTaskConfiguration;

        public WrappedGeneratorContainerBuilder(GeneratorContainerBuilder generatorContainerBuilder, GenerateFeatureFileCodeBehindTaskConfiguration generateFeatureFileCodeBehindTaskConfiguration)
        {
            _generatorContainerBuilder = generatorContainerBuilder;
            _generateFeatureFileCodeBehindTaskConfiguration = generateFeatureFileCodeBehindTaskConfiguration;
        }

        public IObjectContainer BuildGeneratorContainer(
            SpecFlowConfigurationHolder specFlowConfigurationHolder,
            ProjectSettings projectSettings,
            IReadOnlyCollection<GeneratorPluginInfo> generatorPluginInfos,
            IObjectContainer rootObjectContainer)
        {
            var objectContainer = _generatorContainerBuilder.CreateContainer(specFlowConfigurationHolder, projectSettings, generatorPluginInfos, rootObjectContainer);

            objectContainer.RegisterTypeAs<ProjectCodeBehindGenerator, IProjectCodeBehindGenerator>();
            objectContainer.RegisterTypeAs<AnalyticsEventProvider, IAnalyticsEventProvider>();
            objectContainer.RegisterTypeAs<MSBuildTraceListener, ITraceListener>();

            if (_generateFeatureFileCodeBehindTaskConfiguration.OverrideFeatureFileCodeBehindGenerator is null)
            {
                objectContainer.RegisterTypeAs<FeatureFileCodeBehindGenerator, IFeatureFileCodeBehindGenerator>();
            }
            else
            {
                objectContainer.RegisterInstanceAs(_generateFeatureFileCodeBehindTaskConfiguration.OverrideFeatureFileCodeBehindGenerator);
            }

            objectContainer.Resolve<IConfigurationLoader>().TraceConfigSource(objectContainer.Resolve<ITraceListener>(), objectContainer.Resolve<SpecFlowConfiguration>());

            return objectContainer;
        }
    }
}
