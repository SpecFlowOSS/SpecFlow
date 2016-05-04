using System;
using System.CodeDom;
using System.Collections.Generic;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MbUnit3TestGeneratorProvider : MbUnitTestGeneratorProvider
    {
        public MbUnit3TestGeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override UnitTestGeneratorTraits GetTraits()
        {
            return base.GetTraits() | UnitTestGeneratorTraits.ParallelExecution;
        }

        public override void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, TEST_ATTR);
            CodeDomHelper.AddAttribute(testMethod, DESCRIPTION_ATTR, scenarioTitle);
        }

        public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            featureCategories = AddMbUnitParallelizableAttribute(generationContext.TestClass, featureCategories);
            base.SetTestClassCategories(generationContext, featureCategories);
        }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            scenarioCategories= AddMbUnitParallelizableAttribute(testMethod, scenarioCategories);
            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);
        }

        private IEnumerable<string> AddMbUnitParallelizableAttribute(CodeTypeMember codeTypeMember, IEnumerable<string> categories)
        {
            var mbunitParallelizableAttibuteType = "MbUnit.Framework.Parallelizable";
            var mbunitParallelizableAttibuteTag = "Parallelizable";

            var categoriesList = new List<string>();

            foreach (var category in categories)
            {
                if (string.Equals(category, mbunitParallelizableAttibuteTag, StringComparison.InvariantCultureIgnoreCase))
                {
                    CodeDomHelper.AddAttribute(codeTypeMember, mbunitParallelizableAttibuteType);
                }
                else
                {
                    categoriesList.Add(category);
                }
            }

            return categoriesList;
        }
    }
}