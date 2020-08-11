using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.CSharp;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.UnitTestProvider
{
    public class XUnit2TestGeneratorProviderTests
    {
        private const string XUnitInlineDataAttribute = "Xunit.InlineDataAttribute";
        private const string SampleFeatureFile = @"
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen";

        [Fact]
        public void XUnit2TestGeneratorProvider_OnlyVariantName_ShouldSetInlineDataAttributesCorrectly()
        {
            // ARRANGE
            const string sampleFeatureFile = @"
              Feature: Sample feature file

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
            var sampleTestGeneratorProvider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp), new ProjectSettings() { DefaultNamespace = "Target"});
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var inlineDataAttributes = code.Class()
                                           .Members()
                                           .Single(m => m.Name == "SimpleScenarioOutline")
                                           .CustomAttributes()
                                           .Where(a => a.Name == XUnitInlineDataAttribute)
                                           .ToArray();

            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "something");
            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "something else");
        }

        [Fact]
        public void XUnit2TestGeneratorProvider_ExamplesFirstColumnIsDifferentAndMultipleColumns_ShouldSetDescriptionCorrectly()
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
            var sampleTestGeneratorProvider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp), new ProjectSettings() { DefaultNamespace = "Target" });
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var inlineDataAttributes = code.Class()
                                           .Members()
                                           .Single(m => m.Name == "SimpleScenarioOutline")
                                           .CustomAttributes()
                                           .Where(a => a.Name == XUnitInlineDataAttribute)
                                           .ToArray();

            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "something");
            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "something else");
        }

        [Fact]
        public void XUnit2TestGeneratorProvider_ExampleIdentifier_ShouldSetInlineDataAttributesCorrectly()
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
            var sampleTestGeneratorProvider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp), new ProjectSettings() { DefaultNamespace = "Target" });
            var converter = sampleTestGeneratorProvider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            var inlineDataAttributes = code.Class()
                                           .Members()
                                           .Single(m => m.Name == "SimpleScenarioOutline")
                                           .CustomAttributes()
                                           .Where(a => a.Name == XUnitInlineDataAttribute)
                                           .ToArray();

            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "something");
            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "something else");
            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "another");
            inlineDataAttributes.Should().ContainSingle(attribute => attribute.ArgumentValues().First() as string == "and another");
        }

        [Fact]
        public void XUnit2TestGeneratorProvider_ShouldSetDisplayNameForTheoryAttribute()
        {
            // Arrange
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider()), new ProjectSettings() { DefaultNamespace = "Target" });
            var context = new Generator.TestClassGenerationContext(
                unitTestGeneratorProvider: null,
                document: new SpecFlowDocument(
                    feature: new SpecFlowFeature(
                        tags: null,
                        location: null,
                        language: null,
                        keyword: null,
                        name: "",
                        description: null,
                        children: null
                        ),
                    comments: null,
                    documentLocation: null),
                ns: null,
                testClass: null,
                testRunnerField: null,
                testClassInitializeMethod: null,
                testClassCleanupMethod: null,
                testInitializeMethod: null,
                testCleanupMethod: null,
                scenarioInitializeMethod: null,
                scenarioStartMethod:null,
                scenarioCleanupMethod: null,
                featureBackgroundMethod: null,
                generateRowTests: false);
            var codeMemberMethod = new CodeMemberMethod();

            // Act
            provider.SetRowTest(context, codeMemberMethod, "Foo");

            // Assert
            var modifiedAttribute = codeMemberMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == "Xunit.SkippableTheoryAttribute");

            modifiedAttribute.Should().NotBeNull();


            var attribute = modifiedAttribute.Arguments
                                             .OfType<CodeAttributeArgument>()
                                             .FirstOrDefault(a => a.Name == "DisplayName");

            attribute.Should().NotBeNull();
            

            var primitiveExpression = attribute.Value as CodePrimitiveExpression;

            primitiveExpression.Should().NotBeNull();
            primitiveExpression.Value.Should().Be("Foo");
        }

        [Fact]
        public void XUnit2TestGeneratorProvider_ShouldSetSkipAttributeForTheory()
        {
            // Arrange
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider()), new ProjectSettings() { DefaultNamespace = "Target" });

            // Act
            var codeMemberMethod = new CodeMemberMethod
            {
                CustomAttributes =
                    new CodeAttributeDeclarationCollection(
                        new[] { new CodeAttributeDeclaration(XUnit2TestGeneratorProvider.THEORY_ATTRIBUTE) })
            };
            provider.SetTestMethodIgnore(null, codeMemberMethod);

            // Assert
            var modifiedAttribute = codeMemberMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                                                    .FirstOrDefault(a => a.Name == XUnit2TestGeneratorProvider.THEORY_ATTRIBUTE);

            modifiedAttribute.Should().NotBeNull();


            var attribute = modifiedAttribute.Arguments
                                             .OfType<CodeAttributeArgument>()
                                             .FirstOrDefault(a => a.Name == XUnit2TestGeneratorProvider.THEORY_ATTRIBUTE_SKIP_PROPERTY_NAME);

            attribute.Should().NotBeNull();


            var primitiveExpression = attribute.Value as CodePrimitiveExpression;

            primitiveExpression.Should().NotBeNull();
            primitiveExpression.Value.Should().Be(XUnit2TestGeneratorProvider.SKIP_REASON);
        }


        /*
         * Based on w1ld's `Should_set_skip_attribute_for_theory`,
         * refactor as appropriate.
         */

        [Fact]
        public void XUnit2TestGeneratorProvider_ShouldSetDisplayNameForFactAttribute()
        {
            // Arrange
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider()), new ProjectSettings() { DefaultNamespace = "Target" });
            var context = new Generator.TestClassGenerationContext(
                unitTestGeneratorProvider: null,
                document: new Parser.SpecFlowDocument(
                    feature: new SpecFlowFeature(
                        tags: null,
                        location: null,
                        language: null,
                        keyword: null,
                        name: "",
                        description: null,
                        children: null
                        ),
                    comments: null,
                    documentLocation: null),
                ns: null,
                testClass: null,
                testRunnerField: null,
                testClassInitializeMethod: null,
                testClassCleanupMethod: null,
                testInitializeMethod: null,
                testCleanupMethod: null,
                scenarioInitializeMethod: null,
                scenarioStartMethod: null,
                scenarioCleanupMethod: null,
                featureBackgroundMethod: null,
                generateRowTests: false);
            var codeMemberMethod = new CodeMemberMethod();

            // Act
            provider.SetTestMethod(context, codeMemberMethod, "Foo");

            // Assert
            var modifiedAttribute = codeMemberMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == "Xunit.SkippableFactAttribute");

            modifiedAttribute.Should().NotBeNull();

            var attribute = modifiedAttribute.Arguments
                                             .OfType<CodeAttributeArgument>()
                                             .FirstOrDefault(a => a.Name == "DisplayName");

            attribute.Should().NotBeNull();


            var primitiveExpression = attribute.Value as CodePrimitiveExpression;

            primitiveExpression.Should().NotBeNull();
            primitiveExpression.Value.Should().Be("Foo");
        }

        [Fact]
        public void XUnit2TestGeneratorProvider_ShouldInitializeTestOutputHelperFieldInConstructor()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFile);
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp), new ProjectSettings() { DefaultNamespace = "Target" });
            var converter = provider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            code.Should().NotBeNull();
            
            // ASSERT
            var classConstructor = code.Class().Members().Single(m => m.Name == ".ctor");
            classConstructor.Should().NotBeNull();
            classConstructor.Parameters.Count.Should().Be(3);
            classConstructor.Parameters[2].Type.BaseType.Should().Be("Xunit.Abstractions.ITestOutputHelper");
            classConstructor.Parameters[2].Name.Should().Be("testOutputHelper");

            var initOutputHelper = classConstructor.Statements.OfType<CodeAssignStatement>().First();
            initOutputHelper.Should().NotBeNull();
            ((CodeFieldReferenceExpression)initOutputHelper.Left).FieldName.Should().Be("_testOutputHelper");
            ((CodeVariableReferenceExpression)initOutputHelper.Right).VariableName.Should().Be("testOutputHelper");
        }

        [Fact]
        public void XUnit2TestGeneratorProvider_Should_add_testOutputHelper_field_in_class()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFile);
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp), new ProjectSettings() { DefaultNamespace = "Target" });
            var converter = provider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");
            
            // ASSERT
            code.Should().NotBeNull();
            var loggerInstance = code.Class().Members.OfType<CodeMemberField>().First(m => m.Name == @"_testOutputHelper");
            loggerInstance.Type.BaseType.Should().Be("Xunit.Abstractions.ITestOutputHelper");
            loggerInstance.Attributes.Should().Be(MemberAttributes.Private | MemberAttributes.Final);
        }

        [Fact]
        public void XUnit2TestGeneratorProvider_Should_register_testOutputHelper_on_scenario_setup()
        {
            // ARRANGE
            var document = ParseDocumentFromString(SampleFeatureFile);
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp), new ProjectSettings() { DefaultNamespace = "Target" });
            var converter = provider.CreateUnitTestConverter();

            // ACT
            var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

            // ASSERT
            code.Should().NotBeNull();
            var scenarioStartMethod = code.Class().Members().Single(m => m.Name == @"ScenarioInitialize");
            scenarioStartMethod.Statements.Count.Should().Be(2);

            var expression = scenarioStartMethod.Statements[1].Should().BeOfType<CodeExpressionStatement>()
                                                .Which.Expression.Should().BeOfType<CodeMethodInvokeExpression>().Which;
            expression.Parameters[0].Should().BeOfType<CodeVariableReferenceExpression>()
                      .Which.VariableName.Should().Be("_testOutputHelper");

            var method = expression.Method;
            method.TargetObject.Should().BeOfType<CodePropertyReferenceExpression>()
                  .Which.PropertyName.Should().Be("ScenarioContainer");
            method.MethodName.Should().Be("RegisterInstanceAs");
            method.TypeArguments.Should().NotBeNullOrEmpty();
            method.TypeArguments[0].BaseType.Should().Be("Xunit.Abstractions.ITestOutputHelper");
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
