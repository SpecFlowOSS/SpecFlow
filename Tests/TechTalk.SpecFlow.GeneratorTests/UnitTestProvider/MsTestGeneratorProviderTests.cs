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
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenOnlyVariantName()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                var document = parser.Parse(reader, null);
                Assert.NotNull(document);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.NotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
                var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
            }
        }

        [Fact]
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenVariantNameFirstColumnIsTheSame()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFileSameFirstColumn))
            {
                var document = parser.Parse(reader, null);
                Assert.NotNull(document);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.NotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Variant0").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: Variant 0");
                var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Variant1").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: Variant 1");
            }
        }

        [Fact]
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenVariantNameFirstColumnIsDifferentAndMultipleColumns()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFileMultipleColumns))
            {
                var document = parser.Parse(reader, null);
                Assert.NotNull(document);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.NotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
                var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
            }
        }

        [Fact]
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenExampleSetIdentifierIsUsed()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFileWithMultipleExampleSets))
            {
                var document = parser.Parse(reader, null);
                Assert.NotNull(document);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.NotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet0_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
                var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet0_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
                var descriptionAttributeForThirdScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet1_Another").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForThirdScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: another");
                var descriptionAttributeForFourthScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_ExampleSet1_AndAnother").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFourthScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: and another");
            }
        }

        [Fact]
        public void MsTestGeneratorShouldInvokeFeatureSetupMethodWithGlobalNamespaceAlias()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFileWithMultipleExampleSets))
            {
                var document = parser.Parse(reader, null);
                Assert.NotNull(document);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.NotNull(code);
                var featureSetupCall = code
                    .Class()
                    .Members()
                    .Single(m => m.Name == "TestInitialize")
                    .Statements
                    .OfType<CodeConditionStatement>()
                    .First()
                    .TrueStatements
                    .OfType<CodeExpressionStatement>()
                    .First()
                    .Expression
                    .As<CodeMethodInvokeExpression>();

                featureSetupCall.Should().NotBeNull();
                featureSetupCall.Method.MethodName.Should().Be("FeatureSetup");
                featureSetupCall.Method.TargetObject.As<CodeTypeReferenceExpression>().Type.Options.Should().Be(CodeTypeReferenceOptions.GlobalReference);
            }
        }
    }
}
