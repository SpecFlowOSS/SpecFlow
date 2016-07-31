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
            return new UnitTestFeatureGenerator(testGeneratorProvider, codeDomHelper,
                                                 new GeneratorConfiguration { AllowRowTests = true, AllowDebugGeneratedFiles = true },
                                                 new DecoratorRegistryStub());
        }
    }
}