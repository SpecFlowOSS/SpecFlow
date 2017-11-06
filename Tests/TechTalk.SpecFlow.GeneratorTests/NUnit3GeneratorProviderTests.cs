using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;

using FluentAssertions;

using NUnit.Framework;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
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

        private const string SampleFeatureFileWithTags = @"
            @tag1 @tag2 @tag3
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen";

        private const string IgnoredFeatureFile = @"
            @ignore
            Feature: Ignored feature file";

        private const string FeatureFileWithIgnoredScenario = @"
            Feature: Feature With Ignored Scenario

            @ignore
            Scenario: Ignored scenario";

        private const string FeatureFileWithParallelizeIgnore = @"
            @externalDependencies @externalDependencies2
            Feature: Parallelized feature file";

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

        private static CodeNamespace GenerateCodeNamespaceFromFeature(string feature,bool parallelCode=false,string[] ignoreParallelTags=null)
        {
            CodeNamespace code;
            using (var reader = new StringReader(feature))
            {
                SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
                SpecFlowDocument document = parser.Parse(reader, "test.feature");

                var featureGenerator = CreateFeatureGenerator(parallelCode,ignoreParallelTags);

                code = featureGenerator.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");
            }

            return code;
        }

        private static IFeatureGenerator CreateFeatureGenerator(bool parallelCode,string[] ignoreParallelTags)
        {
            var container = GeneratorContainerBuilder.CreateContainer(new SpecFlowConfigurationHolder(ConfigSource.Default, null), new ProjectSettings());
            var specFlowConfiguration = container.Resolve<SpecFlowConfiguration>();
            specFlowConfiguration.MarkFeaturesParallelizable = parallelCode;
            specFlowConfiguration.SkipParallelizableMarkerForTags = ignoreParallelTags ??
                                                                      Enumerable.Empty<string>().ToArray();
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

        [Test]
        public void ShouldAddParallelizableAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFileWithTags, true);

            var attributes = code.Class().CustomAttributes().ToArray();
            var parallelAttributes = attributes.Where(a => a.Name == "NUnit.Framework.ParallelizableAttribute").ToList();
            parallelAttributes.Should().HaveCount(1, "Only one Parallelizable attribute should be set");
        }

        [Test]
        public void ShouldAddParallelizableAttributeBecauseThereIsNoMatchingIgnoreTag()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile, true, new[] { "myOtherexternalDependencies" });

            var attributes = code.Class().CustomAttributes().ToArray();
            var attribute = attributes.FirstOrDefault(a => a.Name == "NUnit.Framework.ParallelizableAttribute");

            attribute.Should().NotBeNull("Parallelizable attribute was not found");
        }

        [Test]
        public void ShouldNotAddParallelizableAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(FeatureFileWithParallelizeIgnore, true,new [] { "externalDependencies" });

            var attributes = code.Class().CustomAttributes().ToArray();
            var attribute = attributes.FirstOrDefault(a => a.Name == "NUnit.Framework.ParallelizableAttribute");

            attribute.Should().BeNull("Parallelizable attribute was found");
        }


        [Test]
        public void ShouldProvideAReasonForIgnoringAFeature()
        {
            var code = GenerateCodeNamespaceFromFeature(IgnoredFeatureFile);

            var attributes = code.Class().CustomAttributes().ToArray();
            var attribute = attributes.FirstOrDefault(a => a.Name == "NUnit.Framework.IgnoreAttribute");

            attribute.Should().NotBeNull("Ignore attribute was not found");

            attribute.ArgumentValues()
                .Single()
                .As<string>()
                .Should()
                .NotBeNullOrWhiteSpace("No reason for ignoring the feature was given");
        }

        [Test]
        public void ShouldProvideAReasonForIgnoringAScenario()
        {
            var code = GenerateCodeNamespaceFromFeature(FeatureFileWithIgnoredScenario);

            var attributes = code.Class().Members().ToArray().Single(m => m.Name == "IgnoredScenario").CustomAttributes().ToArray();
            var attribute = attributes.FirstOrDefault(a => a.Name == "NUnit.Framework.IgnoreAttribute");

            attribute.Should().NotBeNull("Ignore attribute was not found");

            attribute.ArgumentValues()
                .Single()
                .As<string>()
                .Should()
                .NotBeNullOrWhiteSpace("No reason for ignoring the scenario was given");
        }
    }
}