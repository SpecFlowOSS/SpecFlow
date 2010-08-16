using System.CodeDom;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    // TODO: it's an ugly copy-paste of MsTestGeneratorProvider - we should consider refactoring the generator creation to support custom factories.
    public class MsTestSilverlightGeneratorProvider : IUnitTestGeneratorProvider
    {
        private const string TESTFIXTURE_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute";
        private const string TEST_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute";
        private const string PROPERTY_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute";
        private const string TESTFIXTURESETUP_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute";
        private const string TESTFIXTURETEARDOWN_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute";
        private const string TESTSETUP_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute";
        private const string TESTTEARDOWN_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute";
        private const string IGNORE_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.IgnoreAttribute";
        private const string DESCRIPTION_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute";
        private const string CATEGORY_ATTR = "Microsoft.Silverlight.Testing.TagAttribute";
        
        private const string FEATURE_TITILE_PROPERTY_NAME = "FeatureTitle";
        private const string FEATURE_TITILE_KEY = "FeatureTitle";

        private const string TESTCONTEXT_TYPE = "Microsoft.VisualStudio.TestTools.UnitTesting.TestContext";

        private CodeTypeDeclaration currentTestTypeDeclaration = null;

        public virtual void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description)
        {
            typeDeclaration.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTFIXTURE_ATTR)));

            //as in mstest, you cannot mark classes with the description attribute, we
            //just remember the feature title, and we will apply it for each test method
            typeDeclaration.UserData[FEATURE_TITILE_KEY] = title;

            currentTestTypeDeclaration = typeDeclaration;
        }

        private void SetDescription(CodeAttributeDeclarationCollection customAttributes, string description)
        {
            customAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(DESCRIPTION_ATTR),
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(description))));
        }

        private void SetProperty(CodeAttributeDeclarationCollection customAttributes, string name, string value)
        {
            customAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(PROPERTY_ATTR),
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(name)),
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(value))));
        }

        public virtual void SetTestFixtureCategories(CodeTypeDeclaration typeDeclaration, IEnumerable<string> categories)
        {
            //MsTest does not support caregories... :(
        }

        public virtual void SetTest(CodeMemberMethod memberMethod, string title)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TEST_ATTR)));

            SetDescription(memberMethod.CustomAttributes, title);

            if (currentTestTypeDeclaration == null)
                return;

            string featureTitle = currentTestTypeDeclaration.UserData[FEATURE_TITILE_KEY] as string;
            if (featureTitle != null)
                SetProperty(memberMethod.CustomAttributes, FEATURE_TITILE_PROPERTY_NAME, featureTitle);
        }

        public virtual void SetTestCategories(CodeMemberMethod memberMethod, IEnumerable<string> categories)
        {
            SetCategories(memberMethod.CustomAttributes, categories);
        }
        
        public virtual void SetTestSetup(CodeMemberMethod memberMethod)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTSETUP_ATTR)));
        }

        public virtual void SetTestFixtureSetup(CodeMemberMethod memberMethod)
        {
            memberMethod.Attributes |= MemberAttributes.Static;

            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTFIXTURESETUP_ATTR)));
        }

        public virtual void SetTestFixtureTearDown(CodeMemberMethod memberMethod)
        {
            memberMethod.Attributes |= MemberAttributes.Static;

            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTFIXTURETEARDOWN_ATTR)));
        }

        public virtual void SetTestTearDown(CodeMemberMethod memberMethod)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTTEARDOWN_ATTR)));
        }

        public virtual void SetIgnore(CodeTypeMember codeTypeMember)
        {
            codeTypeMember.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(IGNORE_ATTR)));
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