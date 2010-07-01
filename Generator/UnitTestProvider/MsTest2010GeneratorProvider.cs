using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTest2010GeneratorProvider : MsTestGeneratorProvider
    {
        private const string CATEGORY_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute";

        public override void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description)
        {
            base.SetTestFixture(typeDeclaration, title, description);

            featureCategories = null;
        }

        private IEnumerable<string> featureCategories = null;

        public override void SetTestFixtureCategories(CodeTypeDeclaration typeDeclaration, IEnumerable<string> categories)
        {
            featureCategories = categories.ToArray();
        }

        public override void SetTest(CodeMemberMethod memberMethod, string title)
        {
            base.SetTest(memberMethod, title);
            if (featureCategories != null)
                SetCategories(memberMethod.CustomAttributes, featureCategories);
        }

        public override void SetTestCategories(CodeMemberMethod memberMethod, IEnumerable<string> categories)
        {
            SetCategories(memberMethod.CustomAttributes, categories);
        }

        private void SetCategories(CodeAttributeDeclarationCollection customAttributes, IEnumerable<string> categories)
        {
            foreach (var category in categories)
            {
                customAttributes.Add(
                    new CodeAttributeDeclaration(
                        new CodeTypeReference(CATEGORY_ATTR),
                        new CodeAttributeArgument(
                            new CodePrimitiveExpression(category))));
            }
        }

    }
}