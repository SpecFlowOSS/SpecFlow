using System;
using System.CodeDom;
using System.Collections.Generic;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestWinRTGeneratorProvider : IUnitTestGeneratorProvider
    {
        protected const string TESTFIXTURE_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestClassAttribute";
        protected const string TEST_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestMethodAttribute";
        protected const string PROPERTY_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestPropertyAttribute";
        protected const string TESTFIXTURESETUP_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassInitializeAttribute";
        protected const string TESTFIXTURETEARDOWN_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.ClassCleanupAttribute";
        protected const string TESTSETUP_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestInitializeAttribute";
        protected const string TESTTEARDOWN_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestCleanupAttribute";
        protected const string IGNORE_ATTR = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.IgnoreAttribute";
        protected const string DESCRIPTION_ATTR = "System.ComponentModel.DescriptionAttribute";

        protected const string FEATURE_TITILE_PROPERTY_NAME = "FeatureTitle";

        protected const string TESTCONTEXT_TYPE = "Microsoft.VisualStudio.TestPlatform.UnitTestFramework.TestContext";

        protected CodeDomHelper CodeDomHelper { get; set; }

        public virtual bool SupportsRowTests { get { return false; } }
        public virtual bool SupportsAsyncTests { get { return false; } }

        public MsTestWinRTGeneratorProvider(CodeDomHelper codeDomHelper)
        {
            CodeDomHelper = codeDomHelper;
        }

        private void SetProperty(CodeTypeMember codeTypeMember, string name, string value)
        {
            CodeDomHelper.AddAttribute(codeTypeMember, PROPERTY_ATTR, name, value);
        }

        public virtual void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, TESTFIXTURE_ATTR);
        }

        public virtual void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            //MsTest does not support caregories... :(
        }

        public void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, IGNORE_ATTR);
        }

        public virtual void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            // by default, doing nothing to the final generated code
        }


        public virtual void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            generationContext.TestClassInitializeMethod.Attributes |= MemberAttributes.Static;
            generationContext.TestClassInitializeMethod.Parameters.Add(new CodeParameterDeclarationExpression(
                TESTCONTEXT_TYPE, "testContext"));

            CodeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TESTFIXTURESETUP_ATTR);
        }

        public void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            generationContext.TestClassCleanupMethod.Attributes |= MemberAttributes.Static;
            CodeDomHelper.AddAttribute(generationContext.TestClassCleanupMethod, TESTFIXTURETEARDOWN_ATTR);
        }


        public virtual void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestInitializeMethod, TESTSETUP_ATTR);

            //if (FeatureContext.Current != null && FeatureContext.Current.FeatureInfo.Title != "<current_feature_title>")
            //  <TestClass>.<TestClassInitialize>(null);

            FixTestRunOrderingIssue(generationContext);
        }

        protected virtual void FixTestRunOrderingIssue(TestClassGenerationContext generationContext)
        {
            //see https://github.com/techtalk/SpecFlow/issues/96
            generationContext.TestInitializeMethod.Statements.Add(
                new CodeConditionStatement(
                    new CodeBinaryOperatorExpression(
                        new CodeBinaryOperatorExpression(
                            new CodePropertyReferenceExpression(
                                new CodeTypeReferenceExpression(typeof (FeatureContext)),
                                "Current"),
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(null)),
                        CodeBinaryOperatorType.BooleanAnd,
                        new CodeBinaryOperatorExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(
                                    new CodePropertyReferenceExpression(
                                        new CodeTypeReferenceExpression(typeof (FeatureContext)),
                                        "Current"),
                                    "FeatureInfo"),
                                "Title"),
                            CodeBinaryOperatorType.IdentityInequality,
                            new CodePrimitiveExpression(generationContext.Feature.Title))),
                    new CodeExpressionStatement(
                        new CodeMethodInvokeExpression(
                            new CodeTypeReferenceExpression(
                                generationContext.Namespace.Name + "." + generationContext.TestClass.Name
                                ),
                            generationContext.TestClassInitializeMethod.Name,
                            new CodePrimitiveExpression(null)))));
        }

        public void SetTestCleanupMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestCleanupMethod, TESTTEARDOWN_ATTR);
        }


        public virtual void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, scenarioTitle);

            //as in mstest, you cannot mark classes with the description attribute, we
            //just apply it for each test method as a property
            SetProperty(testMethod, FEATURE_TITILE_PROPERTY_NAME, generationContext.Feature.Title);
        }

        public virtual void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            //MsTest does not support caregories... :(
        }

        public void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, IGNORE_ATTR);
        }


        public virtual void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            //MsTest does not support row tests... :(
            throw new NotSupportedException();
        }

        public virtual void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored)
        {
            //MsTest does not support row tests... :(
            throw new NotSupportedException();
        }


        public virtual void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments)
        {
            if (!string.IsNullOrEmpty(exampleSetName))
            {
                SetProperty(testMethod, "ExampleSetName", exampleSetName);
            }

            if (!string.IsNullOrEmpty(variantName))
            {
                SetProperty(testMethod, "VariantName", variantName);
            }

            foreach (var pair in arguments)
            {
                SetProperty(testMethod, "Parameter:" + pair.Key, pair.Value);
            }
        }

        public void SetTestRunner(TestClassGenerationContext generationContext, CodeExpression testRunnerField)
        {
            // Add System.Reflection import - required for GetTypeInfo extension method
            generationContext.Namespace.Imports.Add(new CodeNamespaceImport("System.Reflection"));
            
            generationContext.TestClassInitializeMethod.Statements.Add(new CodeVariableDeclarationStatement(typeof(System.Reflection.Assembly), "assembly"));

            // typeof(ApplicationHasTestsFeature).GetType().GetTypeInfo().Assembly
            var assemblyField = new CodeVariableReferenceExpression("assembly");

            generationContext.TestClassInitializeMethod.Statements.Add(
                new CodeAssignStatement(
                    assemblyField,
                    new CodePropertyReferenceExpression(new CodeMethodInvokeExpression(new CodeTypeOfExpression(new CodeTypeReference(generationContext.TestClass.Name)),"GetTypeInfo"),"Assembly")));

            var methodName = generationContext.GenerateAsynchTests ? "GetAsyncTestRunner" : "GetTestRunner";
            generationContext.TestClassInitializeMethod.Statements.Add(
                new CodeAssignStatement(
                    testRunnerField,
                    new CodeMethodInvokeExpression(
                        new CodeTypeReferenceExpression(typeof(TestRunnerManager)),
                        methodName, assemblyField)));
        }
    }
}