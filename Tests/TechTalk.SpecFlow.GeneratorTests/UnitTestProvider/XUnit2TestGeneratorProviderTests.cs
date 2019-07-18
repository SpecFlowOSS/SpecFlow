using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;
using FluentAssertions;
using Microsoft.CSharp;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests.UnitTestProvider
{
    public class XUnit2TestGeneratorProviderTests
    {
        private const string SampleFeatureFile = @"
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen";

        
        [Fact]
        public void Should_set_displayname_theory_attribute()
        {
            // Arrange
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider()));
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
                    sourceFilePath: null),
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
                .FirstOrDefault(a => a.Name == "Xunit.TheoryAttribute");

            modifiedAttribute.Should().NotBeNull();

            
            var attribute = modifiedAttribute.Arguments.OfType<CodeAttributeArgument>()
                .FirstOrDefault(a => a.Name == "DisplayName");

            attribute.Should().NotBeNull();
            

            var primitiveExpression = attribute.Value as CodePrimitiveExpression;

            primitiveExpression.Should().NotBeNull();
            primitiveExpression.Value.Should().Be("Foo");
        }

        [Fact]
        public void Should_set_skip_attribute_for_theory()
        {
            // Arrange
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider())); // TODO: what about XUnit2TestGeneratorProvider ?

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


            var attribute = modifiedAttribute.Arguments.OfType<CodeAttributeArgument>()
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
        public void Should_set_displayname_attribute()
        {
            // Arrange
            var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider())); // TODO: what about XUnit2TestGeneratorProvider ?
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
                    sourceFilePath: null),
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
                .FirstOrDefault(a => a.Name == "Xunit.FactAttribute");

            modifiedAttribute.Should().NotBeNull();

            var attribute = modifiedAttribute.Arguments.OfType<CodeAttributeArgument>()
                .FirstOrDefault(a => a.Name == "DisplayName");

            attribute.Should().NotBeNull();


            var primitiveExpression = attribute.Value as CodePrimitiveExpression;

            primitiveExpression.Should().NotBeNull();
            primitiveExpression.Value.Should().Be("Foo");
        }

        [Fact]
        public void Should_initialize_testOutputHelper_field_in_constructor()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                var document = parser.Parse(reader, null);
                document.Should().NotBeNull();
                

                var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = provider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                code.Should().NotBeNull();
                
                var classContructor = code.Class().Members().Single(m => m.Name == ".ctor");
                classContructor.Should().NotBeNull();
                classContructor.Parameters.Count.Should().Be(2);
                classContructor.Parameters[1].Type.BaseType.Should().Be("Xunit.Abstractions.ITestOutputHelper");
                classContructor.Parameters[1].Name.Should().Be("testOutputHelper");

                var initOutputHelper = classContructor.Statements.OfType<CodeAssignStatement>().First();
                initOutputHelper.Should().NotBeNull();
                ((CodeFieldReferenceExpression)(initOutputHelper.Left)).FieldName.Should().Be("_testOutputHelper");
                ((CodeVariableReferenceExpression)(initOutputHelper.Right)).VariableName.Should().Be("testOutputHelper");
            }
        }
        
        [Fact]
        public void Should_add_testOutputHelper_field_in_class()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                var document = parser.Parse(reader, null);
                document.Should().NotBeNull();

                var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = provider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");
                
                code.Should().NotBeNull();
                var loggerInstance = code.Class().Members.OfType<CodeMemberField>().First(m => m.Name == @"_testOutputHelper");
                loggerInstance.Type.BaseType.Should().Be("Xunit.Abstractions.ITestOutputHelper");
                loggerInstance.Attributes.Should().Be(MemberAttributes.Private | MemberAttributes.Final);
            }
        }

        [Fact]
        public void Should_register_testOutputHelper_on_scenario_setup()
        {
            var parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                var document = parser.Parse(reader, null);
                document.Should().NotBeNull();

                var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = provider.CreateUnitTestConverter();
                var code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                code.Should().NotBeNull();
                var scenarioStartMethod = code.Class().Members().Single(m => m.Name == @"ScenarioInitialize");

                scenarioStartMethod.Statements.Count.Should().Be(2);
                var expression = (scenarioStartMethod.Statements[1] as CodeExpressionStatement).Expression;
                var method = (expression as CodeMethodInvokeExpression).Method;
                (method.TargetObject as CodePropertyReferenceExpression).PropertyName.Should().Be("ScenarioContainer");
                method.MethodName.Should().Be("RegisterInstanceAs");
                method.TypeArguments.Should().NotBeNullOrEmpty();
                method.TypeArguments[0].BaseType.Should().Be("Xunit.Abstractions.ITestOutputHelper");

                ((expression as CodeMethodInvokeExpression).Parameters[0] as CodeVariableReferenceExpression).VariableName.Should().Be("_testOutputHelper");
            }
        }

    }
}
