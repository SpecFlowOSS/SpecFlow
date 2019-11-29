using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

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

            if (_generateFeatureFileCodeBehindTaskConfiguration.OverrideFeatureFileCodeBehindGenerator is null)
            {
                objectContainer.RegisterTypeAs<FeatureFileCodeBehindGenerator, IFeatureFileCodeBehindGenerator>();
            }
            else
            {
                objectContainer.RegisterInstanceAs(_generateFeatureFileCodeBehindTaskConfiguration.OverrideFeatureFileCodeBehindGenerator);
            }

            return objectContainer;
        }
    }
}
