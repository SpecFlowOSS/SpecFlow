using System.CodeDom;
using System.Linq;

using Microsoft.CSharp;

using NUnit.Framework;

using TechTalk.SpecFlow.Generator.UnitTestProvider;
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
    }
}