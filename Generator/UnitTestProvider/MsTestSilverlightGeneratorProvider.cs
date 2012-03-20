using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Async;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestSilverlightGeneratorProvider : MsTestGeneratorProvider
    {
        private const string TAG_ATTR = "Microsoft.Silverlight.Testing.TagAttribute";
        private const string SILVERLIGHTTEST_BASE = "Microsoft.Silverlight.Testing.SilverlightTest";
        private const string ASYNCTEST_ATTR = "Microsoft.Silverlight.Testing.AsynchronousAttribute";
        private const string ASYNCTEST_INTERFACE = "TechTalk.SpecFlow.Async.ISilverlightTestInstance";

        public override bool SupportsAsyncTests { get { return true; } }

        public MsTestSilverlightGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            base.SetTestClass(generationContext, featureTitle, featureDescription);

            generationContext.TestClass.BaseTypes.Add(new CodeTypeReference(SILVERLIGHTTEST_BASE));
        }

        public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            var categories = featureCategories.ToArray();
            CodeDomHelper.AddAttributeForEachValue(generationContext.TestClass, TAG_ATTR, categories);
        }

        public override void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            generationContext.TestClassInitializeMethod.Attributes |= MemberAttributes.Static;

            CodeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TESTFIXTURESETUP_ATTR);
        }

        protected override void FixTestRunOrderingIssue(TestClassGenerationContext generationContext)
        {
            //nop; hopefully the issue is not present on Silverlight
        }

        public override void SetTestInitializeMethod(TestClassGenerationContext generationContext)
        {
            base.SetTestInitializeMethod(generationContext);

            // SenarioContext.Current.SetTestInstance(this);
            var scenarioContext = new CodeTypeReferenceExpression("ScenarioContext");
            var currentContext = new CodePropertyReferenceExpression(scenarioContext, "Current");
            var scenarioContextExtensions = new CodeTypeReferenceExpression("ScenarioContextExtensions");
            var setTestInstance = new CodeMethodInvokeExpression(scenarioContextExtensions, "SetTestInstance",
                currentContext, new CodeThisReferenceExpression());

            // Add it to ScenarioSetup
            generationContext.ScenarioInitializeMethod.Statements.Add(new CodeExpressionStatement(setTestInstance));
        }

        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            base.SetTestMethod(generationContext, testMethod, scenarioTitle);

            if (generationContext.GenerateAsynchTests)
                SetupAsyncTest(testMethod);
        }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(testMethod, TAG_ATTR, scenarioCategories);
        }

        public override void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            base.FinalizeTestClass(generationContext);

            if (generationContext.GenerateAsynchTests)
                SetupAsyncTestClass(generationContext);
        }

        private void SetupAsyncTestClass(TestClassGenerationContext generationContext)
        {
            generationContext.TestClass.BaseTypes.Add(new CodeTypeReference(ASYNCTEST_INTERFACE));

            //AsyncTestRunner.RegisterAsyncTestExecutor(testRunner, new TechTalk.SpecFlow.Async.SilverlightAsyncTestExecutor(this));

            var nawSilverlightAsyncTestExecutorExpr = new CodeObjectCreateExpression("TechTalk.SpecFlow.Async.SilverlightAsyncTestExecutor", new CodeThisReferenceExpression());

            var registerAsyncExpression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(AsyncTestRunner)), 
                "RegisterAsyncTestExecutor", 
                new CodeVariableReferenceExpression("testRunner"),
                nawSilverlightAsyncTestExecutorExpr);

            generationContext.TestInitializeMethod.Statements.Add(new CodeExpressionStatement(registerAsyncExpression));
        }

        private void SetupAsyncTest(CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, ASYNCTEST_ATTR);
        }
    }
}