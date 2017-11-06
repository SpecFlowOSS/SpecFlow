using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
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
    }
}