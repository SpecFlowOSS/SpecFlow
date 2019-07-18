﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator.CodeDom;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class MsTestV2GeneratorProvider : MsTestGeneratorProvider
    {
        private const string DONOTPARALLELIZE_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.DoNotParallelize";
        private const string DONOTPARALLELIZE_TAG = "MsTest:donotparallelize";
        private const string CATEGORY_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute";
        private const string OWNER_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.OwnerAttribute";
        private const string WORKITEM_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.WorkItemAttribute";
        private const string DEPLOYMENTITEM_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.DeploymentItemAttribute";
        private const string OWNER_TAG = "owner:";
        private const string WORKITEM_TAG = "workitem:";
        private const string DEPLOYMENTITEM_TAG = "MsTest:deploymentitem:";

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

            generationContext.CustomData["featureCategories"] = Enumerable.ToArray<string>(GetNonMSTestSpecificTags(featureCategories));

            IEnumerable<string> ownerTags = featureCategories.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (ownerTags.Any())
            {
                generationContext.CustomData[OWNER_TAG] = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
            }

            IEnumerable<string> workItemTags = featureCategories.Where(t => t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (workItemTags.Any())
            {
                int temp;
                IEnumerable<string> workItemsAsStrings = workItemTags.Select(t => t.Substring(WORKITEM_TAG.Length).Trim('\"'));
                if (workItemsAsStrings.Any())
                {
                    generationContext.CustomData[WORKITEM_TAG] = workItemsAsStrings.Where(t => int.TryParse(t, out temp)).Select(t => int.Parse(t));
                }
            }

            IEnumerable<string> deploymentItemTags = featureCategories.Where(t => t.StartsWith(DEPLOYMENTITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (deploymentItemTags.Any())
            {
                IEnumerable<string> deploymentItemsAsStrings = deploymentItemTags.Select(t => t.Substring(DEPLOYMENTITEM_TAG.Length));
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
                if (!String.IsNullOrEmpty(ownerName))
                {
                    CodeDomHelper.AddAttribute(testMethod, OWNER_ATTR, ownerName);
                }
            }

            if (generationContext.CustomData.ContainsKey(WORKITEM_TAG))
            {
                IEnumerable<int> workItems = generationContext.CustomData[WORKITEM_TAG] as IEnumerable<int>;
                foreach (int workItem in workItems)
                {
                    CodeDomHelper.AddAttribute(testMethod, WORKITEM_ATTR, workItem);
                }
            }

            if (generationContext.CustomData.ContainsKey(DEPLOYMENTITEM_TAG))
            {
                IEnumerable<string> deploymentItems = generationContext.CustomData[DEPLOYMENTITEM_TAG] as IEnumerable<string>;
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
            IEnumerable<string> doNotParallelizeTags = scenarioCategories.Where(s => s.StartsWith(DONOTPARALLELIZE_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);

            if (doNotParallelizeTags.Any() && !generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                CodeDomHelper.AddAttribute(testMethod, DONOTPARALLELIZE_ATTR);
            }

            if (doNotParallelizeTags.Any() && generationContext.CustomData.ContainsKey(DONOTPARALLELIZE_TAG))
            {
                scenarioCategories = scenarioCategories.Where(s => !s.StartsWith(DONOTPARALLELIZE_TAG)).ToList();
            }

            IEnumerable<string> tags = scenarioCategories.ToList();

            IEnumerable<string> ownerTags = tags.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (ownerTags.Any())
            {
                string ownerName = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
                if (!String.IsNullOrEmpty(ownerName))
                {
                    CodeDomHelper.AddAttribute(testMethod, OWNER_ATTR, ownerName);
                }
            }

            IEnumerable<string> workItemTags = tags.Where(t => t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
            if (workItemTags.Any())
            {
                int temp;
                IEnumerable<string> workItemsAsStrings = workItemTags.Select(t => t.Substring(WORKITEM_TAG.Length).Trim('\"'));
                IEnumerable<int> workItems = workItemsAsStrings.Where(t => int.TryParse(t, out temp)).Select(t => int.Parse(t));
                foreach (int workItem in workItems)
                {
                    CodeDomHelper.AddAttribute(testMethod, WORKITEM_ATTR, workItem);
                }
            }

            CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, GetNonMSTestSpecificTags(tags));
        }

        private IEnumerable<string> GetNonMSTestSpecificTags(IEnumerable<string> tags)
        {
            return tags == null ? new string[0] : tags.Where(t =>
                                                                 (!t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase))
                                                                 && (!t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase))
                                                                 && (!t.StartsWith(DEPLOYMENTITEM_TAG, StringComparison.InvariantCultureIgnoreCase)))
                                                      .Select(t => t);
        }
    }
}
