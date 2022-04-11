using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.UnitTestProvider
{
    public class MsTestGeneratorProviderTests
    {
        private const string TestDescriptionAttributeName = "Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute";
        private const string SampleFeatureFile = @"
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

        private const string SampleFeatureFileWithMultipleExampleSets = @"
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
        private const string SampleFeatureFileSameFirstColumn = @"
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
        private const string SampleFeatureFileMultipleColumns = @"
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

        [Fact]
        public void MsTestGeneratorProvider_ExampleSetSingleColumn_ShouldSetDescriptionWithVariantNameFromFirstColumn()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFile);
            var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
            var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
        }

        [Fact]
        public void MsTestGeneratorProvider_ExamplesWithIdenticalFirstColumn_ShouldSetDescriptionCorrectly()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFileSameFirstColumn);
            var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Variant0").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: Variant 0");
            var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Variant1").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: Variant 1");
        }

        [Fact]
        public void MsTestGeneratorProvider_ExamplesFirstColumnIsDifferentAndMultipleColumns_ShouldSetDescriptionCorrectly()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFileMultipleColumns);
            var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
            var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
        }

        [Fact]
        public void MsTestGeneratorProvider_ExampleSetIdentifiers_ShouldSetDescriptionCorrectly()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFileWithMultipleExampleSets);
            var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet0_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
            var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet0_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
            var descriptionAttributeForThirdScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet1_Another").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForThirdScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: another");
            var descriptionAttributeForFourthScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet1_AndAnother").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
            descriptionAttributeForFourthScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: and another");
        }

        [Fact]
        public void MsTestGeneratorProvider_ShouldInvokeFeatureSetupMethodWithGlobalNamespaceAlias()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFileWithMultipleExampleSets);
            var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var featureSetupCall = code
                .Class()
                .Members()
                .Single(m => m.Name == "TestInitializeAsync")
                .Statements
                .OfType<CodeConditionStatement>()
                .First()
                .TrueStatements
                .OfType<CodeExpressionStatement>()
                .First()
                .Expression
                .As<CodeMethodInvokeExpression>();

            featureSetupCall.Should().NotBeNull();
            featureSetupCall.Method.MethodName.Should().Be("FeatureSetupAsync");
            featureSetupCall.Method.TargetObject.As<CodeTypeReferenceExpression>().Type.BaseType.Should().Contain("global::");
        }

        [Fact]
        public void MsTestGeneratorProvider_ShouldNotHaveParallelExecutionTrait()
        {
            var sut = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

            sut.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.ParallelExecution)
                .Should()
                .BeFalse("trait ParallelExecution was found");
        }

        [Fact]
        public void MsTestGeneratorProvider_WithFeatureWithMatchingTag_ShouldNotAddDoNotParallelizeAttribute()
        {
            // ARRANGE
            var document = ParseDocumentFromString(@"
            @nonparallelizable
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something");

            var provider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var featureGenerator = provider.CreateFeatureGenerator(addNonParallelizableMarkerForTags: new string[] { "nonparallelizable" });

            // ACT
            var code = featureGenerator.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var attributes = code.Class().CustomAttributes().ToArray();
            attributes.Should().NotContain(a => a.Name == "Microsoft.VisualStudio.TestTools.UnitTesting.DoNotParallelizeAttribute");
        }

        [Fact]
        public void MsTestGeneratorProvider_WithFeatureWithNoMatchingTag_ShouldNotAddDoNotParallelizeAttribute()
        {
            // ARRANGE
            var document = ParseDocumentFromString(@"
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something");

            var provider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var featureGenerator = provider.CreateFeatureGenerator(addNonParallelizableMarkerForTags: new string[] { "nonparallelizable" });

            // ACT
            var code = featureGenerator.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var attributes = code.Class().CustomAttributes().ToArray();
            attributes.Should().NotContain(a => a.Name == "Microsoft.VisualStudio.TestTools.UnitTesting.DoNotParallelizeAttribute");
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
