using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using FluentAssertions;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace GeneratorTests
{
    [TestFixture]
    public class FeatureGeneratorProviderTests
    {
        private static UnitTestFeatureGeneratorProvider CreateUnitTestFeatureGeneratorProvider()
        {
            GeneratorConfiguration generatorConfiguration = new GeneratorConfiguration();
            CodeDomHelper codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            UnitTestFeatureGenerator unitTestFeatureGenerator = new UnitTestFeatureGenerator(new NUnitTestGeneratorProvider(codeDomHelper), codeDomHelper, generatorConfiguration);

            return new UnitTestFeatureGeneratorProvider(unitTestFeatureGenerator);
        }

        [Test]
        public void Should_UnitTestFeatureGeneratorProvider_have_low_priority()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            generatorProvider.Priority.Should().Be(int.MaxValue);
        }

        [Test]
        public void Should_UnitTestFeatureGeneratorProvider_be_able_to_generate_anything()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            Feature anyFeature = new Feature();
            generatorProvider.CanGenerate(anyFeature, "default").Should().Be(true);
        }

        [Test]
        public void Should_UnitTestFeatureGeneratorProvider_create_valid_instance()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            Feature anyFeature = new Feature();
            var generator = generatorProvider.CreateGenerator(anyFeature);

            generator.Should().NotBeNull();
        }

        [Test]
        public void Should_UnitTestFeatureGeneratorProvider_create_UnitTestFeatureGenerator_instance()
        {
            var generatorProvider = CreateUnitTestFeatureGeneratorProvider();
            Feature anyFeature = new Feature();
            var generator = generatorProvider.CreateGenerator(anyFeature);

            generator.Should().BeOfType<UnitTestFeatureGenerator>();
        }
    }
}
