using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MbUnitTestGeneratorProvider : IUnitTestGeneratorProvider, ICodeDomHelperRequired
    {
        private const string TESTFIXTURE_ATTR = "MbUnit.Framework.TestFixtureAttribute";
        private const string TEST_ATTR = "MbUnit.Framework.TestAttribute";
        private const string ROWTEST_ATTR = "MbUnit.Framework.RowTestAttribute";
        private const string ROW_ATTR = "MbUnit.Framework.RowAttribute";
        private const string CATEGORY_ATTR = "MbUnit.Framework.CategoryAttribute";
        private const string TESTSETUP_ATTR = "MbUnit.Framework.SetUpAttribute";
        private const string TESTFIXTURESETUP_ATTR = "MbUnit.Framework.FixtureSetUpAttribute";
        private const string TESTFIXTURETEARDOWN_ATTR = "MbUnit.Framework.FixtureTearDownAttribute";
        private const string TESTTEARDOWN_ATTR = "MbUnit.Framework.TearDownAttribute";
        private const string IGNORE_ATTR = "MbUnit.Framework.IgnoreAttribute";
        private const string DESCRIPTION_ATTR = "MbUnit.Framework.DescriptionAttribute";

        public CodeDomHelper CodeDomHelper { get; set; }

        public bool SupportsRowTests { get { return true; } }
        public bool SupportsAsyncTests { get { return false; } }

        public void SetTestFixture(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, TESTFIXTURE_ATTR);
            CodeDomHelper.AddAttribute(generationContext.TestClass, DESCRIPTION_ATTR, featureDescription);
        }

        public void SetTestFixtureCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(generationContext.TestClass, CATEGORY_ATTR, featureCategories);
        }

        public void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, IGNORE_ATTR);
        }

        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            // by default, doing nothing to the final generated code
        }


        public void SetTestFixtureSetup(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TESTFIXTURESETUP_ATTR);
        }

        public void SetTestFixtureTearDown(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassCleanupMethod, TESTFIXTURETEARDOWN_ATTR);
        }


        public void SetTestSetup(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestInitializeMethod, TESTSETUP_ATTR);
        }

        public void SetTestTearDown(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestCleanupMethod, TESTTEARDOWN_ATTR);
        }


        public void SetTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, scenarioTitle);
        }

        public void SetTestCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, scenarioCategories);
        }

        public void SetIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, IGNORE_ATTR);
        }


        public void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, ROWTEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, scenarioTitle);
        }

        public void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            //TODO: better handle "ignored"
            if (isIgnored)
                return;

            var args = arguments.Select(
              arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            args.Add(
                new CodeAttributeArgument(
                    new CodeArrayCreateExpression(typeof(string[]), tags.Select(t => new CodePrimitiveExpression(t)).ToArray())));

            CodeDomHelper.AddAttribute(testMethod, ROW_ATTR, args.ToArray());
        }

        public void SetTestVariant(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            // doing nothing since we support RowTest
        }
    }
}