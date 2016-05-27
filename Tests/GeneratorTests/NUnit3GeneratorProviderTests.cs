using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;

using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class NUnit3GeneratorProviderTests
    {
        private const string SampleFeatureFile = @"
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen";

        [Test]
        public void ShouldNotGenerateObsoleteTestFixtureSetUpAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureSetup");

            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.TestFixtureSetUpAttribute")
                .Should()
                .BeNull();
        }

        private static CodeNamespace GenerateCodeNamespaceFromFeature(string feature)
        {
            CodeNamespace code;
            using (var reader = new StringReader(feature))
            {
                SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
                SpecFlowDocument document = parser.Parse(reader, "test.feature");

                var featureGenerator = CreateFeatureGenerator();

                code = featureGenerator.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");
            }

            return code;
        }

        private static IFeatureGenerator CreateFeatureGenerator()
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(), new ProjectSettings());
            container.RegisterInstanceAs(CreateTestGeneratorProvider());

            var generator = container.Resolve<UnitTestFeatureGeneratorProvider>().CreateGenerator(ParserHelper.CreateAnyDocument());
            return generator;
        }

        private static NUnit3TestGeneratorProvider CreateTestGeneratorProvider()
        {
            return new NUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
        }

        [Test]
        public void ShouldGenerateNewOneTimeSetUpAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureSetup");

            // Assert that we do use the NUnit3 attribute
            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.OneTimeSetUpAttribute")
                .Should()
                .NotBeNull();
        }

        [Test]
        public void ShouldNotGenerateObsoleteTestFixtureTearDownAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureTearDown");

            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.TestFixtureTearDownAttribute")
                .Should()
                .BeNull();
        }

        [Test]
        public void ShouldGenerateNewOneTimeTearDownAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureTearDown");

            // Assert that we do use the NUnit3 attribute
            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.OneTimeTearDownAttribute")
                .Should()
                .NotBeNull();
        }

        [Test]
        public void ShouldHaveRowTestsTrait()
        {
            var nUnit3TestGeneratorProvider = CreateTestGeneratorProvider();

            nUnit3TestGeneratorProvider.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.RowTests)
                .Should()
                .BeTrue("trait RowTests was not found");
        }

        [Test]
        public void ShouldHaveParallelExecutionTrait()
        {
            var nUnit3TestGeneratorProvider = CreateTestGeneratorProvider();

            nUnit3TestGeneratorProvider.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.ParallelExecution)
                .Should()
                .BeTrue("trait ParallelExecution was not found");
        }
    }
}