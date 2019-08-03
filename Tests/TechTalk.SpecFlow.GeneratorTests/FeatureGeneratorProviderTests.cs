using Xunit;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using FluentAssertions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Generation;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace TechTalk.SpecFlow.GeneratorTests
{
    
    public class FeatureGeneratorProviderTests
    {
        private static UnitTestFeatureGeneratorProvider CreateUnitTestFeatureGeneratorProvider()
        {
            Configuration.SpecFlowConfiguration generatorSpecFlowConfiguration = ConfigurationLoader.GetDefault();
            CodeDomHelper codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            UnitTestFeatureGenerator unitTestFeatureGenerator = new UnitTestFeatureGenerator(
                new NUnit3TestGeneratorProvider(codeDomHelper), codeDomHelper, generatorSpecFlowConfiguration, new DecoratorRegistryStub());

            return new UnitTestFeatureGeneratorProvider(unitTestFeatureGenerator);
        }

        [Fact]
        public void Should_UnitTestFeatureGeneratorProvider_have_low_priority()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            generatorProvider.Priority.Should().Be(int.MaxValue);
        }

        [Fact]
        public void Should_UnitTestFeatureGeneratorProvider_be_able_to_generate_anything()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            var anyFeature = ParserHelper.CreateAnyDocument();
            generatorProvider.CanGenerate(anyFeature).Should().Be(true);
        }

        [Fact]
        public void Should_UnitTestFeatureGeneratorProvider_create_valid_instance()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            var anyFeature = ParserHelper.CreateAnyDocument();
            var generator = generatorProvider.CreateGenerator(anyFeature);

            generator.Should().NotBeNull();
        }

        [Fact]
        public void Should_UnitTestFeatureGeneratorProvider_create_UnitTestFeatureGenerator_instance()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            var anyFeature = ParserHelper.CreateAnyDocument();
            var generator = generatorProvider.CreateGenerator(anyFeature);

            generator.Should().BeOfType<UnitTestFeatureGenerator>();
        }
    }
}
