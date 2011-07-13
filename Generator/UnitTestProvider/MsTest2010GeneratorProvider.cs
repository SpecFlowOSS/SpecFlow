using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTest2010GeneratorProvider : MsTestGeneratorProvider
    {
        private const string CATEGORY_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute";

        public override void SetTestFixtureCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            generationContext.CustomData["featureCategories"] = featureCategories.ToArray();
        }

        public override void SetTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            base.SetTest(generationContext, testMethod, scenarioTitle);
            var featureCategories = (string[]) generationContext.CustomData["featureCategories"];
            if (featureCategories != null)
                CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, featureCategories);
        }

        public override void SetTestCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, scenarioCategories);
        }
    }
}