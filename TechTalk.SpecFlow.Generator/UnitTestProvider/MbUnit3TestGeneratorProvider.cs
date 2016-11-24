using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MbUnit3TestGeneratorProvider : MbUnitTestGeneratorProvider
    {
        protected const string PARRALLELIZABLE_ATTR = "MbUnit.Framework.ParallelizableAttribute";

        public MbUnit3TestGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, scenarioTitle);
        }

        public override UnitTestGeneratorTraits GetTraits()
        {
            return base.GetTraits()| UnitTestGeneratorTraits.ParallelExecution;
        }

        public override void SetTestClassParrallelize(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, PARRALLELIZABLE_ATTR, new CodeAttributeArgument(new CodePrimitiveExpression(generationContext.TestClass.Name)));
        }
    }
}