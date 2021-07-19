using System.Linq;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Generation;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public static class IUnitTestGeneratorProviderExtensions
    {
        public static UnitTestFeatureGenerator CreateUnitTestConverter(this IUnitTestGeneratorProvider testGeneratorProvider)
        {
            var codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);

            var runtimeConfiguration = ConfigurationLoader.GetDefault();
            runtimeConfiguration.AllowRowTests = true;
            runtimeConfiguration.AllowDebugGeneratedFiles = true;

            return new UnitTestFeatureGenerator(testGeneratorProvider, codeDomHelper, runtimeConfiguration, new DecoratorRegistryStub());
        }

        public static IFeatureGenerator CreateFeatureGenerator(this IUnitTestGeneratorProvider testGeneratorProvider, string[] addNonParallelizableMarkerForTags = null)
        {
            var container = new GeneratorContainerBuilder().CreateContainer(new SpecFlowConfigurationHolder(ConfigSource.Default, null), new ProjectSettings(), Enumerable.Empty<GeneratorPluginInfo>());
            var specFlowConfiguration = container.Resolve<SpecFlowConfiguration>();
            specFlowConfiguration.AddNonParallelizableMarkerForTags = addNonParallelizableMarkerForTags;
            container.RegisterInstanceAs(testGeneratorProvider);

            var generator = container.Resolve<UnitTestFeatureGeneratorProvider>().CreateGenerator(ParserHelper.CreateAnyDocument());
            return generator;
        }
    }
}