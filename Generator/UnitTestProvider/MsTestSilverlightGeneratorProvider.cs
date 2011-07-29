using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Async;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestSilverlightGeneratorProvider : MsTestGeneratorProvider
    {
        private const string TAG_ATTR = "Microsoft.Silverlight.Testing.TagAttribute";
        private const string ASYNCTEST_BASE = "Microsoft.Silverlight.Testing.SilverlightTest";
        private const string ASYNCTEST_ATTR = "Microsoft.Silverlight.Testing.AsynchronousAttribute";

        public override bool SupportsAsyncTests { get { return true; } }

        public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            generationContext.CustomData["featureCategories"] = featureCategories.ToArray();
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

        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            base.SetTestMethod(generationContext, testMethod, scenarioTitle);
            if (generationContext.CustomData.ContainsKey("featureCategories"))
            {
                var featureCategories = (string[]) generationContext.CustomData["featureCategories"];
                CodeDomHelper.AddAttributeForEachValue(testMethod, TAG_ATTR, featureCategories);
            }

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
            generationContext.TestClass.BaseTypes.Add(new CodeTypeReference(ASYNCTEST_BASE));
            generationContext.TestClass.BaseTypes.Add(new CodeTypeReference("TechTalk.SpecFlow.Async.ISilverlightTestInstance"));

            //AsyncTestRunner.RegisterAsyncTestExecutor(testRunner, new TechTalk.SpecFlow.Async.SilverlightAsyncTestExecutor(this));

            var nawSilverlightAsyncTestExecutorExpr = new CodeObjectCreateExpression("TechTalk.SpecFlow.Async.SilverlightAsyncTestExecutor", new CodeThisReferenceExpression());

            var registerAsyncExpression = new CodeMethodInvokeExpression(
                new CodeTypeReferenceExpression(typeof(AsyncTestRunner)), 
                "RegisterAsyncTestExecutor", 
                new CodeVariableReferenceExpression("testRunner"),
                nawSilverlightAsyncTestExecutorExpr);

            generationContext.ScenarioInitializeMethod.Statements.Insert(0, new CodeExpressionStatement(registerAsyncExpression));
        }

        private void SetupAsyncTest(CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, ASYNCTEST_ATTR);
        }
    }
}