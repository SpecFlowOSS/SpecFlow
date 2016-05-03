using System.CodeDom;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class MbUnitGeneratorProviderTests
    {
        private const string TestDescriptionAttributeName = "MbUnit.Framework.DescriptionAttribute";
        private const string ParallelizableAttibuteName = "MbUnit.Framework.Parallelizable";
        private const string RowAttributeName=  "MbUnit.Framework.RowAttribute";

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

        [Test]
        public void MbUnitGeneratorShouldSetDescriptionOnceForScenarioOutLine()
        { 
            SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                SpecFlowFeature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MbUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider);
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);
                var descriptionAttributeForFirstScenarioOutline = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline").CustomAttributes().Single(a => a.Name == TestDescriptionAttributeName);
                descriptionAttributeForFirstScenarioOutline.ArgumentValues().First().Should().Be("Simple Scenario Outline");
            }
        }

        [Test]
        public void MbUnitGeneratorShouldSetRowAttributesForScenarioOutLine()
        {
            SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                SpecFlowFeature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MbUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider);
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");
  

                Assert.IsNotNull(code);
                var rowAttributes = code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline").CustomAttributes().Where(a => a.Name == RowAttributeName).ToList();

                rowAttributes.Should().NotBeNullOrEmpty();
                rowAttributes.ElementAt(0).ArgumentValues().Single().Should().Be("something");
                rowAttributes.ElementAt(1).ArgumentValues().Single().Should().Be("something else");

            }
        }

        [Test]
        public void MbUniTestGeneratorShouldAddParallelizableAttributeOnClassWhenParallelizableTagIsUsedAtFeatureLevel()
        {
            SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                SpecFlowFeature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MbUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider,new []{ "Parallelizable" });
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);

                code.Class().CustomAttributes().Should().ContainSingle(a => a.Name == ParallelizableAttibuteName);
            }
        }

        [Test]
        public void MbUniTestGeneratorShouldAddParallelizableAttributeOnTestMethodWhenParallelizableTagIsUsedAtScenarioLevel()
        {
            SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                SpecFlowFeature feature = parser.Parse(reader, null);
                Assert.IsNotNull(feature);

                var sampleTestGeneratorProvider = new MbUnit3TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = CreateUnitTestConverter(sampleTestGeneratorProvider,null, new[] { "Parallelizable" });
                CodeNamespace code = converter.GenerateUnitTestFixture(feature, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);

                code.Class().Members().Single(m => m.Name == "SimpleScenarioOutline").CustomAttributes().Should().ContainSingle(a => a.Name == ParallelizableAttibuteName); ;
            }
        }

        public static UnitTestFeatureGenerator CreateUnitTestConverter(IUnitTestGeneratorProvider testGeneratorProvider,IEnumerable<string> featureTags=null,IEnumerable<string> scenarioTags=null)
        {
            var codeDomHelper = new CodeDomHelper(CodeDomProviderLanguage.CSharp);
            return new UnitTestFeatureGenerator(testGeneratorProvider, codeDomHelper,
                                                 new GeneratorConfiguration { AllowRowTests = true, AllowDebugGeneratedFiles = true },
                                                 new DecoratorRegistryStub(featureTags??new List<string>(),scenarioTags??new List<string>()));
        }
    }
}