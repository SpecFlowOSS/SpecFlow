using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator.CodeDom;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestV2GeneratorProvider : MsTestGeneratorProvider
    {
        protected internal const string DONOTPARALLELIZE_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.DoNotParallelizeAttribute";
        protected internal const string DONOTPARALLELIZE_TAG = "MsTest:donotparallelize";
        protected internal const string CATEGORY_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute";
        protected internal const string OWNER_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.OwnerAttribute";
        protected internal const string WORKITEM_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.WorkItemAttribute";
        protected internal const string DEPLOYMENTITEM_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute";
        protected internal const string OWNER_TAG = "owner:";
        protected internal const string WORKITEM_TAG = "workitem:";
        protected internal const string DEPLOYMENTITEM_TAG = "MsTest:deploymentitem:";

        public MsTestV2GeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
        {
        }

        public override UnitTestGeneratorTraits GetTraits()
        {
            return UnitTestGeneratorTraits.ParallelExecution;
        }

        public override void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription)
        {
            if (generationContext.Feature.Tags.Any(t => t.Name.Substring(1).StartsWith(DEPLOYMENTITEM_TAG, StringComparison.InvariantCultureIgnoreCase)))
            {
                CodeDomHelper.AddAttribute(generationContext.TestClass, DEPLOYMENTITEM_ATTR, "TechTalk.SpecFlow.MSTest.SpecFlowPlugin.dll");
            }

            base.SetTestClass(generationContext, featureTitle, featureDescription);
        }

        public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
        {
            var doNotParallelizeTags = featureCategories.Where(f => f.StartsWith(DONOTPARALLELIZE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (doNotParallelizeTags.Any())
            {
               generationContext.CustomData[DONOTPARALLELIZE_TAG] = string.Empty;
            }

            generationContext.CustomData["featureCategories"] = GetNonMSTestSpecificTags(featureCategories).ToArray();

            var ownerTags = featureCategories.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (ownerTags.Any())
            {
                generationContext.CustomData[OWNER_TAG] = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
            }

            var workItemTags = featureCategories.Where(t => t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (workItemTags.Any())
            {
                int temp;
                var workItemsAsStrings = workItemTags.Select(t => t.Substring(WORKITEM_TAG.Length).Trim('\"'));
                if (workItemsAsStrings.Any())
                {
                    generationContext.CustomData[WORKITEM_TAG] = workItemsAsStrings.Where(t => int.TryParse(t, out temp)).Select(t => int.Parse(t));
                }
            }

            var deploymentItemTags = featureCategories.Where(t => t.StartsWith(DEPLOYMENTITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (deploymentItemTags.Any())
            {
                var deploymentItemsAsStrings = deploymentItemTags.Select(t => t.Substring(DEPLOYMENTITEM_TAG.Length));
                if (deploymentItemsAsStrings.Any())
                {
                    generationContext.CustomData[DEPLOYMENTITEM_TAG] = deploymentItemsAsStrings;
                }
            }
        }

        public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName)
        {
            if (generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                CodeDomHelper.AddAttribute(testMethod, DONOTPARALLELIZE_ATTR);
            }

            base.SetTestMethod(generationContext, testMethod, friendlyTestName);
            if (generationContext.CustomData.ContainsKey("featureCategories"))
            {
                var featureCategories = (string[])generationContext.CustomData["featureCategories"];
                CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, featureCategories);
            }

            if (generationContext.CustomData.ContainsKey(OWNER_TAG))
            {
                string ownerName = generationContext.CustomData[OWNER_TAG] as string;
                if (!string.IsNullOrEmpty(ownerName))
                {
                    CodeDomHelper.AddAttribute(testMethod, OWNER_ATTR, ownerName);
                }
            }

            if (generationContext.CustomData.ContainsKey(WORKITEM_TAG))
            {
                var workItems = generationContext.CustomData[WORKITEM_TAG] as IEnumerable<int>;
                foreach (int workItem in workItems)
                {
                    CodeDomHelper.AddAttribute(testMethod, WORKITEM_ATTR, workItem);
                }
            }

            if (generationContext.CustomData.ContainsKey(DEPLOYMENTITEM_TAG))
            {
                var deploymentItems = generationContext.CustomData[DEPLOYMENTITEM_TAG] as IEnumerable<string>;
                foreach (string deploymentItem in deploymentItems)
                {
                    var outputDirProvided = deploymentItem.Split(':').Any();
                    if (outputDirProvided)
                    {
                        CodeDomHelper.AddAttribute(testMethod, DEPLOYMENTITEM_ATTR, deploymentItem.Split(':'));
                    }
                    else
                    {
                        CodeDomHelper.AddAttribute(testMethod, DEPLOYMENTITEM_ATTR, deploymentItem);
                    }
                }
            }
        }

        public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
        {
            var scenarioCategoriesArray = scenarioCategories.ToArray();
            var doNotParallelizeTags = scenarioCategoriesArray.Where(s => s.StartsWith(DONOTPARALLELIZE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);

            if (doNotParallelizeTags.Any() && !generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                CodeDomHelper.AddAttribute(testMethod, DONOTPARALLELIZE_ATTR);
            }

            if (doNotParallelizeTags.Any() && generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                scenarioCategoriesArray = scenarioCategoriesArray.Where(s => !s.StartsWith(DONOTPARALLELIZE_TAG)).ToArray();
            }

            var ownerTags = scenarioCategoriesArray.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (ownerTags.Any())
            {
                string ownerName = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
                if (!string.IsNullOrEmpty(ownerName))
                {
                    CodeDomHelper.AddAttribute(testMethod, OWNER_ATTR, ownerName);
                }
            }

            var workItemTags = scenarioCategoriesArray.Where(t => t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (workItemTags.Any())
            {
                var workItemsAsStrings = workItemTags.Select(t => t.Substring(WORKITEM_TAG.Length).Trim('\"'));
                var workItems = workItemsAsStrings.Where(t => int.TryParse(t, out _)).Select(int.Parse);
                foreach (int workItem in workItems)
                {
                    CodeDomHelper.AddAttribute(testMethod, WORKITEM_ATTR, workItem);
                }
            }

            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, GetNonMSTestSpecificTags(scenarioCategoriesArray));
        }

        public override void SetTestClassNonParallelizable(TestClassGenerationContext generationContext)
        {
            CodeDomHelper.AddAttribute(generationContext.TestClass, DONOTPARALLELIZE_ATTR);
        }

        private IEnumerable<string> GetNonMSTestSpecificTags(IEnumerable<string> tags)
        {
            return tags == null ? new string[0] : tags.Where(t => !t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase))
                                                      .Where(t => !t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase))
                                                      .Where(t => !t.StartsWith(DEPLOYMENTITEM_TAG, StringComparison.InvariantCultureIgnoreCase))
                                                      .Select(t => t);
        }
    }
}
