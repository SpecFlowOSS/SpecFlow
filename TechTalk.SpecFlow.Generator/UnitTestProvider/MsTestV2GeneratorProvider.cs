using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestV2GeneratorProvider : MsTest2010GeneratorProvider
    {
        private const string DONOTPARALLELIZE_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.DoNotParallelize";
        private const string DONOTPARALLELIZE_TAG = "MsTest:donotparallelize";

        public MsTestV2GeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.ParallelExecution;
        }

        public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            IEnumerable<string> doNotParallelizeTags = featureCategories.Where(f => f.StartsWith(DONOTPARALLELIZE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (doNotParallelizeTags.Any())
            {
               generationContext.CustomData[DONOTPARALLELIZE_TAG] = String.Empty;
            }
            base.SetTestClassCategories(generationContext, featureCategories);
        }

        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
           

            if (generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                CodeDomHelper.AddAttribute(testMethod, DONOTPARALLELIZE_ATTR);
            }
            base.SetTestMethod(generationContext, testMethod, friendlyTestName);
        }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            

            IEnumerable<string> doNotParallelizeTags = scenarioCategories.Where(s => s.StartsWith(DONOTPARALLELIZE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);

            if (doNotParallelizeTags.Any() && !generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                CodeDomHelper.AddAttribute(testMethod, DONOTPARALLELIZE_ATTR);
            }

            if (doNotParallelizeTags.Any() && generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                scenarioCategories = scenarioCategories.Where(s => !s.StartsWith(DONOTPARALLELIZE_TAG)).ToList();
            }

            base.SetTestMethodCategories(generationContext, testMethod, scenarioCategories);
        }
    }
}