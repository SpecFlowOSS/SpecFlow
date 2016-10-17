using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class NUnitTestGeneratorProvider : IUnitTestGeneratorProvider
    {
        protected const string TESTFIXTURE_ATTR = "NUnit.Framework.TestFixtureAttribute";
        protected const string TEST_ATTR = "NUnit.Framework.TestAttribute";
        protected const string ROW_ATTR = "NUnit.Framework.TestCaseAttribute";
        protected const string CATEGORY_ATTR = "NUnit.Framework.CategoryAttribute";
        protected const string TESTSETUP_ATTR = "NUnit.Framework.SetUpAttribute";
        protected const string TESTFIXTURESETUP_ATTR = "NUnit.Framework.TestFixtureSetUpAttribute";
        protected const string TESTFIXTURETEARDOWN_ATTR = "NUnit.Framework.TestFixtureTearDownAttribute";
        protected const string TESTTEARDOWN_ATTR = "NUnit.Framework.TearDownAttribute";
        protected const string IGNORE_ATTR = "NUnit.Framework.IgnoreAttribute";
        protected const string DESCRIPTION_ATTR = "NUnit.Framework.DescriptionAttribute";

        protected CodeDomHelper CodeDomHelper { get; set; }

        public virtual UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.RowTests;
        }

        public NUnitTestGeneratorProvider(CodeDomHelper codeDomHelper)
        {
            CodeDomHelper = codeDomHelper;
        }

        public void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, TESTFIXTURE_ATTR);
            CodeDomHelper.AddAttribute(generationContext.TestClass, DESCRIPTION_ATTR, featureTitle);
        }

        public void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(generationContext.TestClass, CATEGORY_ATTR, featureCategories);
        }

        public virtual void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, IGNORE_ATTR);
        }

        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            // by default, doing nothing to the final generated code
        }


        public virtual void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TESTFIXTURESETUP_ATTR);
        }

        public virtual void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassCleanupMethod, TESTFIXTURETEARDOWN_ATTR);
        }


        public void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestInitializeMethod, TESTSETUP_ATTR);
        }

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestCleanupMethod, TESTTEARDOWN_ATTR);
        }


        public void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, friendlyTestName);
        }

        public void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, scenarioCategories);
        }

        public virtual void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, IGNORE_ATTR);
        }


        public void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            SetTestMethod(generationContext, testMethod, scenarioTitle);
        }

        public void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            var args = arguments.Select(
              arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            // addressing ReSharper bug: TestCase attribute with empty string[] param causes inconclusive result - https://github.com/techtalk/SpecFlow/issues/116
            bool hasExampleTags = tags.Any();
            var exampleTagExpressionList = tags.Select(t => new CodePrimitiveExpression(t));
            CodeExpression exampleTagsExpression = hasExampleTags
                ? new CodeArrayCreateExpression(typeof(string[]), exampleTagExpressionList.ToArray())
                : (CodeExpression) new CodePrimitiveExpression(null);
                
            args.Add(new CodeAttributeArgument(exampleTagsExpression));

            // adds 'Category' named parameter so that NUnit also understands that this test case belongs to the given categories
            if (hasExampleTags)
            {
                CodeExpression exampleTagsStringExpr = new CodePrimitiveExpression(string.Join(",", tags.ToArray()));
                args.Add(new CodeAttributeArgument("Category", exampleTagsStringExpr));
            }

            if (isIgnored)
                args.Add(new CodeAttributeArgument("Ignored", new CodePrimitiveExpression(true)));

            CodeDomHelper.AddAttribute(testMethod, ROW_ATTR, args.ToArray());
        }

        public void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            // doing nothing since we support RowTest
        }
    }
}