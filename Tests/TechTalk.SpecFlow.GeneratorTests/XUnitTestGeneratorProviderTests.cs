using System.CodeDom;
using System.Linq;
using FluentAssertions;
using Microsoft.CSharp;
using TechTalk.SpecFlow.Generator.CodeDom;
using Xunit;
using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.GeneratorTests
{
    
    public class XUnitTestGeneratorProviderTests
    {
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
    }
}