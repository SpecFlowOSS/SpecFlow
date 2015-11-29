using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class MSestGeneratorProviderTests
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

        [Test]
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenOnlyVariantName()
        {
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                Feature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
         
                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider);
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");
                
                Assert.IsNotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
                var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
            }
        }

        [Test]
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenVariantNameFirstColumnIsTheSame()
        {
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFileSameFirstColumn))
            {
                Feature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider);
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Variant0").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: Variant 0");
                var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Variant1").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: Variant 1");
            }
        }

        [Test]
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenVariantNameFirstColumnIsDifferentAndMultipleColumns()
        {
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFileMultipleColumns))
            {
                Feature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider);
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_Something").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something");
                var descriptionAttributeForSecondScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline_SomethingElse").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForSecondScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline: something else");
            }
        }

        [Test]
        public void MsTestGeneratorShouldSetDescriptionCorrectlyWhenExampleSetIdentifierIsUsed()
        {
            SpecFlowLangParser parser = new SpecFlowLangParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFileWithMultipleExampleSets))
            {
                Feature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MsTestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider);
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);
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
       
        public static UnitTestFeatureGenerator CreateUnitTestConverter(IUnitTestGeneratorProvider testGeneratorProvider)
        {
            var codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            return new UnitTestFeatureGenerator(testGeneratorProvider, codeDomHelper,
                                                 new GeneratorConfiguration { AllowRowTests = true, AllowDebugGeneratedFiles = true },
                                                 new DecoratorRegistryStub());
        }
    }
}