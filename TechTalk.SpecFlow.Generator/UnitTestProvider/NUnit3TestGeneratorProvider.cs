using System;
using System.CodeDom;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class NUnit3TestGeneratorProvider : NUnitTestGeneratorProvider
    {
        protected const string TESTFIXTURESETUP_ATTR_NUNIT3 = "NUnit.Framework.OneTimeSetUpAttribute";
        protected const string TESTFIXTURETEARDOWN_ATTR_NUNIT3 = "NUnit.Framework.OneTimeTearDownAttribute";
        protected const string PARALLELIZABLE_ATTR = "NUnit.Framework.ParallelizableAttribute";

        public NUnit3TestGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.RowTests | UnitTestGeneratorTraits.ParallelExecution;
        }

        public override void SetTestClassIgnore(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, IGNORE_ATTR, "Ignored feature");
        }

        public override void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod)
        {
            CodeDomHelper.AddAttribute(testMethod, IGNORE_ATTR, "Ignored scenario");
        }

        public override void SetTestClassInitializeMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassInitializeMethod, TESTFIXTURESETUP_ATTR_NUNIT3);
        }

        public override void SetTestClassCleanupMethod(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClassCleanupMethod, TESTFIXTURETEARDOWN_ATTR_NUNIT3);
        }

        public override void SetTestClassParallelize(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, PARALLELIZABLE_ATTR, new CodeAttributeArgument(new CodePrimitiveExpression(generationContext.TestClass.Name)));
        }
    }
}