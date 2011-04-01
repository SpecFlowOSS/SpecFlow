using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class NUnitTestConverter : IUnitTestGeneratorProvider
    {
        private const string TESTFIXTURE_ATTR = "NUnit.Framework.TestFixtureAttribute";
        private const string TEST_ATTR = "NUnit.Framework.TestAttribute";
        private const string ROW_ATTR = "NUnit.Framework.TestCaseAttribute";
        private const string CATEGORY_ATTR = "NUnit.Framework.CategoryAttribute";
        private const string TESTSETUP_ATTR = "NUnit.Framework.SetUpAttribute";
        private const string TESTFIXTURESETUP_ATTR = "NUnit.Framework.TestFixtureSetUpAttribute";
        private const string TESTFIXTURETEARDOWN_ATTR = "NUnit.Framework.TestFixtureTearDownAttribute";
        private const string TESTTEARDOWN_ATTR = "NUnit.Framework.TearDownAttribute";
        private const string IGNORE_ATTR = "NUnit.Framework.IgnoreAttribute";
        private const string DESCRIPTION_ATTR = "NUnit.Framework.DescriptionAttribute";

        public bool SupportsRowTests { get { return true; } }

        public void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description)
        {
            typeDeclaration.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTFIXTURE_ATTR)));

            SetDescription(typeDeclaration.CustomAttributes, title);
        }

        private void SetDescription(CodeAttributeDeclarationCollection customAttributes, string description)
        {
            customAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(DESCRIPTION_ATTR),
                    new CodeAttributeArgument(
                        new CodePrimitiveExpression(description))));
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

        public void SetTestFixtureCategories(CodeTypeDeclaration typeDeclaration, IEnumerable<string> categories)
        {
            SetCategories(typeDeclaration.CustomAttributes, categories);
        }

        public void SetTest(CodeMemberMethod memberMethod, string title)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TEST_ATTR)));

            SetDescription(memberMethod.CustomAttributes, title);
        }
        
        public void SetRowTest(CodeMemberMethod memberMethod, string title)
        {
            SetTest(memberMethod, title);
        }

        public void SetRow(CodeMemberMethod memberMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            var args = arguments.Select(
              arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            args.Add(
                new CodeAttributeArgument(
                    new CodeArrayCreateExpression(typeof(string[]), tags.Select(t => new CodePrimitiveExpression(t)).ToArray())));

            if (isIgnored)
                args.Add(new CodeAttributeArgument("Ignored", new CodePrimitiveExpression(true)));

            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(ROW_ATTR),
                    args.ToArray()));
        }

        public void SetTestCategories(CodeMemberMethod memberMethod, IEnumerable<string> categories)
        {
            SetCategories(memberMethod.CustomAttributes, categories);
        }

        public void SetTestSetup(CodeMemberMethod memberMethod)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTSETUP_ATTR)));
        }

        public void SetTestFixtureSetup(CodeMemberMethod memberMethod)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTFIXTURESETUP_ATTR)));
        }

        public void SetTestFixtureTearDown(CodeMemberMethod memberMethod)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTFIXTURETEARDOWN_ATTR)));
        }

        public void SetTestTearDown(CodeMemberMethod memberMethod)
        {
            memberMethod.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(TESTTEARDOWN_ATTR)));
        }

        public void SetIgnore(CodeTypeMember codeTypeMember)
        {
            codeTypeMember.CustomAttributes.Add(
                new CodeAttributeDeclaration(
                    new CodeTypeReference(IGNORE_ATTR)));
        }


        public virtual void FinalizeTestClass(CodeNamespace codeNameSpace)
        {
            // by default, doing nothing to the final generated code
            return;
        }

        public void SetTestVariant(CodeMemberMethod memberMethod, string title, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            // doing nothing since we support RowTest
            return;
        }
    }
}