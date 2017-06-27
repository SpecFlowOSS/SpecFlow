using System.CodeDom;
using System.Globalization;
using System.IO;
using System.Linq;

using NUnit.Framework;
using Microsoft.CSharp;

using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

using FluentAssertions;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class XUnit2TestGeneratorProviderTests
    {
        private const string SampleFeatureFile = @"
            Feature: Sample feature file

            Scenario: Simple scenario
                Given there is something
                When I do something
                Then something should happen";

        
        [Test]
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
                scenarioCleanupMethod: null,
                featureBackgroundMethod: null,
                generateRowTests: false);
            var codeMemberMethod = new CodeMemberMethod();

            // Act
            provider.SetRowTest(context, codeMemberMethod, "Foo");

            // Assert
            var modifiedAttribute = codeMemberMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == "Xunit.TheoryAttribute");

            Assert.That(modifiedAttribute, Is.Not.Null);
            var attribute = modifiedAttribute.Arguments.OfType<CodeAttributeArgument>()
                .FirstOrDefault(a => a.Name == "DisplayName");
            Assert.That(attribute, Is.Not.Null);

            var primitiveExpression = attribute.Value as CodePrimitiveExpression;
            Assert.That(primitiveExpression, Is.Not.Null);
            Assert.That(primitiveExpression.Value, Is.EqualTo("Foo"));
        }

        [Test]
        public void Should_initialize_testOutputHelper_field_in_constructor()
        {
            SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                SpecFlowDocument document = parser.Parse(reader, null);
                Assert.IsNotNull(document);

                var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = provider.CreateUnitTestConverter();
                CodeNamespace code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);
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
        
        [Test]
        public void Should_add_testOutputHelper_field_in_class()
        {
            SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                SpecFlowDocument document = parser.Parse(reader, null);
                Assert.IsNotNull(document);

                var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = provider.CreateUnitTestConverter();
                CodeNamespace code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);
                var loggerInstance = code.Class().Members.OfType<CodeMemberField>().First(m => m.Name == @"_testOutputHelper");
                loggerInstance.Type.BaseType.Should().Be("Xunit.Abstractions.ITestOutputHelper");
                loggerInstance.Attributes.Should().Be(MemberAttributes.Private | MemberAttributes.Final);
            }
        }

        [Test]
        public void Should_register_testOutputHelper_on_scenario_setup()
        {
            SpecFlowGherkinParser parser = new SpecFlowGherkinParser(new CultureInfo("en-US"));
            using (var reader = new StringReader(SampleFeatureFile))
            {
                SpecFlowDocument document = parser.Parse(reader, null);
                Assert.IsNotNull(document);

                var provider = new XUnit2TestGeneratorProvider(new CodeDomHelper(CodeDomProviderLanguage.CSharp));

                var converter = provider.CreateUnitTestConverter();
                CodeNamespace code = converter.GenerateUnitTestFixture(document, "TestClassName", "Target.Namespace");

                Assert.IsNotNull(code);
                var scenarioStartMethod = code.Class().Members().Single(m => m.Name == @"ScenarioSetup");

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
