using System;
using System.CodeDom;
using System.Linq;
using Gherkin.Ast;
using Microsoft.CSharp;

using NUnit.Framework;

using TechTalk.SpecFlow.Generator.UnitTestProvider;
using TechTalk.SpecFlow.Parser;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class XUnitTestGeneratorProviderTests
    {
        [Test]
        public void Should_set_skip_attribute_for_theory()
        {
            // Arrange
            var provider = new XUnitTestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider())); // TODO: what about XUnit2TestGeneratorProvider ?

            // Act
            var codeMemberMethod = new CodeMemberMethod()
                                   {
                                       CustomAttributes =
                                           new CodeAttributeDeclarationCollection(
                                           new[] { new CodeAttributeDeclaration(XUnit2TestGeneratorProvider.THEORY_ATTRIBUTE) })
                                   };
            provider.SetTestMethodIgnore(null, codeMemberMethod);

            // Assert
            var modifiedAttribute = codeMemberMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == XUnitTestGeneratorProvider.THEORY_ATTRIBUTE);

            Assert.That(modifiedAttribute, Is.Not.Null);
            var attribute = modifiedAttribute.Arguments.OfType<CodeAttributeArgument>()
                .FirstOrDefault(a => a.Name == XUnitTestGeneratorProvider.THEORY_ATTRIBUTE_SKIP_PROPERTY_NAME);
            Assert.That(attribute, Is.Not.Null);

            var primitiveExpression = attribute.Value as CodePrimitiveExpression;
            Assert.That(primitiveExpression, Is.Not.Null);
            Assert.That(primitiveExpression.Value, Is.EqualTo(XUnitTestGeneratorProvider.SKIP_REASON));
        }

        /*
         * Based on w1ld's `Should_set_skip_attribute_for_theory`,
         * refactor as appropriate.
         */
        [Test]
        public void Should_set_displayname_for_fact_attribute()
        {
            // Arrange
            var provider = new XUnitTestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider())); // TODO: what about XUnit2TestGeneratorProvider ?
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
                        children:null
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
            provider.SetTestMethod(context, codeMemberMethod, "Foo");

            // Assert
            var modifiedAttribute = codeMemberMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == "Xunit.FactAttribute");

            Assert.That(modifiedAttribute, Is.Not.Null);
            var attribute = modifiedAttribute.Arguments.OfType<CodeAttributeArgument>()
                .FirstOrDefault(a => a.Name == "DisplayName");
            Assert.That(attribute, Is.Not.Null);

            var primitiveExpression = attribute.Value as CodePrimitiveExpression;
            Assert.That(primitiveExpression, Is.Not.Null);
            Assert.That(primitiveExpression.Value, Is.EqualTo("Foo"));
        }

        [Test]
        public void Should_set_displayname_for_theory_attribute()
        {
            // Arrange
            var provider = new XUnitTestGeneratorProvider(new CodeDomHelper(new CSharpCodeProvider())); // TODO: what about XUnit2TestGeneratorProvider ?
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
            provider.SetTestMethod(context, codeMemberMethod, "Bar");

            // Assert
            var modifiedAttribute = codeMemberMethod.CustomAttributes.OfType<CodeAttributeDeclaration>()
                .FirstOrDefault(a => a.Name == "Xunit.Extensions.TheoryAttribute");

            Assert.That(modifiedAttribute, Is.Not.Null);
            var attribute = modifiedAttribute.Arguments.OfType<CodeAttributeArgument>()
                .FirstOrDefault(a => a.Name == "DisplayName");
            Assert.That(attribute, Is.Not.Null);

            var primitiveExpression = attribute.Value as CodePrimitiveExpression;
            Assert.That(primitiveExpression, Is.Not.Null);
            Assert.That(primitiveExpression.Value, Is.EqualTo("Bar"));
        }
    }
}