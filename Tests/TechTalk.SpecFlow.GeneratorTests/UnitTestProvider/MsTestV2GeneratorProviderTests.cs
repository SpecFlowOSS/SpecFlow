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
    public class MsTestV2GeneratorProviderTests
    {
        private const string TestDeploymentItemAttributeName = "Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute";

        [Fact]
        public void MsTestV2GeneratorProvider_WithFeatureWithoutDeploymentItem_GeneratedClassShouldNotIncludeDeploymentItemForPlugin()
        {
            // ARRANGE
            var document = ParseDocumentFromString(@"
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen");
            var sampleTestGeneratorProvider = new MsTestV2GeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            code.Class().CustomAttributes().Any(a => a.Name == TestDeploymentItemAttributeName).Should().BeFalse();
        }

        [Fact]
        public void MsTestV2GeneratorProvider_WithFeatureContainingDeploymentItem_GeneratedClassShouldIncludeDeploymentItemForPlugin()
        {
            // ARRANGE
            var document = ParseDocumentFromString(@"
            @MsTest:DeploymentItem:DeploymentItemTestFile.txt
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen");

            var sampleTestGeneratorProvider = new MsTestV2GeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var deploymentItemAttributeForClass = code.Class().CustomAttributes().Single(a => a.Name == TestDeploymentItemAttributeName);
            deploymentItemAttributeForClass.ArgumentValues().First().Should().Be("TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll");
        }

        [Fact]
        public void MsTestV2GeneratorProvider_ShouldHaveParallelExecutionTrait()
        {
            var sut = new MsTestV2GeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

            sut.GetTraits()
                .HasFlag(UnitTestGeneratorTraits.ParallelExecution)
                .Should()
                .BeTrue("trait ParallelExecution was not found");
        }

        [Fact]
        public void MsTestV2GeneratorProvider_WithFeatureWithMatchingTag_ShouldAddNonParallelizableAttribute()
        {
            // ARRANGE
            var document = ParseDocumentFromString(@"
            @nonparallelizable
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something");

            var provider = new MsTestV2GeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
            var featureGenerator = provider.CreateFeatureGenerator(addNonParallelizableMarkerForTags: new string[] { "nonparallelizable" });

            // ACT
            var code = featureGenerator.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var attributes = code.Class().CustomAttributes().ToArray();
            attributes.Should()
                      .ContainSingle(a => a.Name == "Microsoft.VisualStudio.TestTools.UnitTesting.DoNotParallelizeAttribute")
                      .Which.Arguments.Count.Should()
                      .Be(0);
        }

        [Fact]
        public void MsTestV2GeneratorProvider_WithFeatureWithNoMatchingTag_ShouldNotAddNonParallelizableAttribute()
        {
            // ARRANGE
            var document = ParseDocumentFromString(@"
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something");

            var provider = new MsTestV2GeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));
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
