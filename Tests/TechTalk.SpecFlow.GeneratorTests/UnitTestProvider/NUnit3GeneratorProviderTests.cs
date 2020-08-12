using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.UnitTestProvider
{
    public class NUnit3GeneratorProviderTests
    {
        private const string NUnit3TestCaseAttributeName = @"NUnit.Framework.TestCaseAttribute";
        private const string SampleFeatureFile = @"
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

        [Fact]
        public void NUnit3TestGeneratorProvider_ExampleSetSingleColumn_ShouldSetDescriptionWithVariantNameFromFirstColumn()
        {
            // ARRANGE
            const string sampleFeatureFile = @"
              Feature: Sample feature file
              
              Scenario: Simple scenario
                  Given there is something
                  When I do something
                  Then something should happen
              
              @mytag
              Scenario Outline: Simple Scenario Outline
                  Given there is something
                      """"""
                        long string
                      """"""
                  When I do <what>
                      | foo | bar |
                      | 1   | 2   |
                  Then something should happen
              Examples:
                  | what           |
                  | something      |
                  | something else |
";

            var document = ParseDocumentFromString(sampleFeatureFile);
            var sampleTestGeneratorProvider = new NUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var testMethod = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "something");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "something else");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ExamplesWithIdenticalFirstColumn_ShouldSetDescriptionCorrectly()
        {
            // ARRANGE
            const string sampleFeatureFileSameFirstColumn = @"
              Feature: Sample feature file
              
              Scenario: Simple scenario
                  Given there is something
                  When I do something
                  Then something should happen
              
              @mytag
              Scenario Outline: Simple Scenario Outline
                  Given there is something
                      """"""
                        long string
                      """"""
                  When I do <what>
                      | foo | bar |
                      | 1   | 2   |
                  Then something should happen
              Examples:
                  | what           | what else       |
                  | something      | thing           |
                  | something      | different thing |
";

            var document = ParseDocumentFromString(sampleFeatureFileSameFirstColumn);
            var sampleTestGeneratorProvider = new NUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var testMethod = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline");

            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName
                                                                      && a.ArgumentValues().OfType<string>().ElementAt(0) == "something"
                                                                      && a.ArgumentValues().OfType<string>().ElementAt(1) == "thing");

            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName
                                                                      && a.ArgumentValues().OfType<string>().ElementAt(0) == "something"
                                                                      && a.ArgumentValues().OfType<string>().ElementAt(1) == "different thing");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ExamplesFirstColumnIsDifferentAndMultipleColumns_ShouldSetDescriptionCorrectly()
        {
            // ARRANGE
            const string sampleFeatureFileMultipleColumns = @"
              Feature: Sample feature file
              
              Scenario: Simple scenario
                  Given there is something
                  When I do something
                  Then something should happen
              
              @mytag
              Scenario Outline: Simple Scenario Outline
                  Given there is something
                      """"""
                        long string
                      """"""
                  When I do <what>
                      | foo | bar |
                      | 1   | 2   |
                  Then something should happen
              Examples:
                  | what           | what else       |
                  | something      | thing           |
                  | something else | different thing |
";

            var document = ParseDocumentFromString(sampleFeatureFileMultipleColumns);
            var sampleTestGeneratorProvider = new NUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var testMethod = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "something");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "something else");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ExampleSetIdentifiers_ShouldSetDescriptionCorrectly()
        {
            // ARRANGE
            const string sampleFeatureFileWithMultipleExampleSets = @"
              Feature: Sample feature file
              
              Scenario: Simple scenario
                  Given there is something
                  When I do something
                  Then something should happen
              
              @mytag
              Scenario Outline: Simple Scenario Outline
                  Given there is something
                      """"""
                        long string
                      """"""
                  When I do <what>
                      | foo | bar |
                      | 1   | 2   |
                  Then something should happen
              Examples:
                  | what           |
                  | something      |
                  | something else |
              Examples:
                  | what           |
                  | another        |
                  | and another    |
";

            var document = ParseDocumentFromString(sampleFeatureFileWithMultipleExampleSets);
            var sampleTestGeneratorProvider = new NUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var testMethod = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "something");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "something else");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "another");
            testMethod.CustomAttributes().Should().ContainSingle(a => a.Name == NUnit3TestCaseAttributeName && (string)a.ArgumentValues().First() == "and another");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldNotGenerateObsoleteTestFixtureSetUpAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureSetup");

            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.TestFixtureSetUpAttribute")
                .Should()
                .BeNull();
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldGenerateNewOneTimeSetUpAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureSetup");

            // Assert that we do use the NUnit3 attribute
            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.OneTimeSetUpAttribute")
                .Should()
                .NotBeNull();
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldNotGenerateObsoleteTestFixtureTearDownAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureTearDown");

            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.TestFixtureTearDownAttribute")
                .Should()
                .BeNull();
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldGenerateNewOneTimeTearDownAttribute()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile);

            var featureSetupMethod = code.Class().Members().Single(m => m.Name == "FeatureTearDown");

            // Assert that we do use the NUnit3 attribute
            featureSetupMethod.CustomAttributes()
                .FirstOrDefault(a => a.Name == "NUnit.Framework.OneTimeTearDownAttribute")
                .Should()
                .NotBeNull();
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldHaveRowTestsTrait()
        {
            var nUnit3TestGeneratorProvider = CreateTestGeneratorProvider();

            nUnit3TestGeneratorProvider.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.RowTests)
                .Should()
                .BeTrue("trait RowTests was not found");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldHaveParallelExecutionTrait()
        {
            var nUnit3TestGeneratorProvider = CreateTestGeneratorProvider();

            nUnit3TestGeneratorProvider.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.ParallelExecution)
                .Should()
                .BeTrue("trait ParallelExecution was not found");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldAddParallelizableAttribute()
        {
            // ARRANGE
            const string exampleFeatureFileWithTags = @"
            @tag1 @tag2 @tag3
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen";

            // ACT
            var code = GenerateCodeNamespaceFromFeature(exampleFeatureFileWithTags, true);

            // ASSERT
            var attributes = code.Class().CustomAttributes().ToArray();
            attributes.Where(a => a.Name == "NUnit.Framework.ParallelizableAttribute")
                      .Should().HaveCount(1, "Only one Parallelizable attribute should be set");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldAddParallelizableAttributeBecauseThereIsNoMatchingIgnoreTag()
        {
            var code = GenerateCodeNamespaceFromFeature(SampleFeatureFile, true, new[] { "myOtherexternalDependencies" });

            var attributes = code.Class().CustomAttributes().ToArray();
            var attribute = attributes.FirstOrDefault(a => a.Name == "NUnit.Framework.ParallelizableAttribute");

            attribute.Should().NotBeNull("Parallelizable attribute was not found");
        }

        [Fact]
        public void NUnit3TestGeneratorProvider_ShouldNotAddParallelizableAttribute()
        {
            const string featureFileWithParallelizeIgnore = @"
            @externalDependencies @externalDependencies2
            Feature: Parallelized feature file";

            var code = GenerateCodeNamespaceFromFeature(featureFileWithParallelizeIgnore, true,new [] { "externalDependencies" });

            var attributes = code.Class().CustomAttributes().ToArray();
            var attribute = attributes.FirstOrDefault(a => a.Name == "NUnit.Framework.ParallelizableAttribute");

            attribute.Should().BeNull("Parallelizable attribute was found");
        }

       

        public CodeNamespace GenerateCodeNamespaceFromFeature(string feature, bool parallelCode = false, string[] ignoreParallelTags = null)
        {
            CodeNamespace code;
            using (var reader = new StringReader(feature))
            {
                var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
                var document = parser.Parse(reader, new SpecFlowDocumentLocation("test.feature"));

                var featureGenerator = CreateFeatureGenerator(parallelCode,ignoreParallelTags);

                code = featureGenerator.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");
            }

            return code;
        }

        public IFeatureGenerator CreateFeatureGenerator(bool parallelCode,string[] ignoreParallelTags)
        {
            var container = new GeneratorContainerBuilder().CreateContainer(new SpecFlowConfigurationHolder(ConfigSource.Default, null), new ProjectSettings(), Enumerable.Empty<GeneratorPluginInfo>());
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

        public SpecFlowDocument ParseDocumentFromString(string documentSource, CultureInfo parserCultureInfo = null)
        {
            var parser = new SpecFlowGherkinParser(parserCultureInfo ?? new CultureInfo("en-US"));
            using (var reader = new StringReader(documentSource))
            {
                var document = parser.Parse(reader, null);
                document.Should().NotBeNull();
                return document;
            }
        }
    }
}
