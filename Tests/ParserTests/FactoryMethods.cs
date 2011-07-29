using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Utils;

namespace ParserTests
{
    public static class FactoryMethods
    {
        public static SpecFlowUnitTestConverter CreateUnitTestConverter(IUnitTestGeneratorProvider testGeneratorProvider)
        {
            var codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            return new SpecFlowUnitTestConverter(testGeneratorProvider, codeDomHelper, 
                                                 new GeneratorConfiguration { AllowRowTests = true, AllowDebugGeneratedFiles = true});
        }
    }
}