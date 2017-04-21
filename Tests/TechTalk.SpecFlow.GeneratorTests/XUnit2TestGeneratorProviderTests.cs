using System.CodeDom;
using System.Linq;

using NUnit.Framework;
using Microsoft.CSharp;

using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Generator.UnitTestProvider;


namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class XUnit2TestGeneratorProviderTests
    {
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
    }
}
