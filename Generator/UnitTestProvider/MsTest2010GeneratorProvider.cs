using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
	public class MsTest2010GeneratorProvider : MsTestGeneratorProvider
	{
		private const string CATEGORY_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute";
		private const string OWNER_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.OwnerAttribute";
		private const string WORKITEM_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.WorkItemAttribute";

		private const string OWNER_TAG = "owner:";
		private const string WORKITEM_TAG = "workitem:";

	    public MsTest2010GeneratorProvider(CodeDomHelper codeDomHelper) : base(codeDomHelper)
	    {
	    }

	    public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
		{
			generationContext.CustomData["featureCategories"] = GetNonMSTestSpecificTags(featureCategories).ToArray();

			IEnumerable<string> ownerTags = featureCategories.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
			if(ownerTags.Any())
			{
				generationContext.CustomData[OWNER_TAG] = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
			}

			IEnumerable<string> workitemTags = featureCategories.Where(t => t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
			if(workitemTags.Any())
			{
				int temp;
				IEnumerable<string> workitemsAsStrings = workitemTags.Select(t => t.Substring(WORKITEM_TAG.Length).Trim('\"'));
				if(workitemsAsStrings.Any())
				{
					generationContext.CustomData[WORKITEM_TAG] = workitemsAsStrings.Where(t => int.TryParse(t, out temp)).Select(t => int.Parse(t));
				}
			}
		}

		public override void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
		{
			base.SetTestMethod(generationContext, testMethod, scenarioTitle);
			if(generationContext.CustomData.ContainsKey("featureCategories"))
			{
				var featureCategories = (string[])generationContext.CustomData["featureCategories"];
				CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, featureCategories);
			}

			if(generationContext.CustomData.ContainsKey(OWNER_TAG))
			{
				string ownerName = generationContext.CustomData[OWNER_TAG] as string;
				if(!String.IsNullOrEmpty(ownerName))
				{
					CodeDomHelper.AddAttribute(testMethod, OWNER_ATTR, ownerName);
				}
			}

			if(generationContext.CustomData.ContainsKey(WORKITEM_TAG))
			{
				IEnumerable<int> workitems = generationContext.CustomData[WORKITEM_TAG] as IEnumerable<int>;
				foreach(int workitem in workitems)
				{
					CodeDomHelper.AddAttribute(testMethod, WORKITEM_ATTR, workitem);
				}
			}
		}

		public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
		{
			IEnumerable<string> tags = scenarioCategories.ToList();

			IEnumerable<string> ownerTags = tags.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
			if(ownerTags.Any())
			{
				string ownerName = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
				if(!String.IsNullOrEmpty(ownerName))
				{
					CodeDomHelper.AddAttribute(testMethod, OWNER_ATTR, ownerName);
				}
			}

			IEnumerable<string> workitemTags = tags.Where(t => t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
			if(workitemTags.Any())
			{
				int temp;
				IEnumerable<string> workitemsAsStrings = workitemTags.Select(t => t.Substring(WORKITEM_TAG.Length).Trim('\"'));
				IEnumerable<int> workitems = workitemsAsStrings.Where(t => int.TryParse(t, out temp)).Select(t => int.Parse(t));
				foreach(int workitem in workitems)
				{
					CodeDomHelper.AddAttribute(testMethod, WORKITEM_ATTR, workitem);
				}
			}

			CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, GetNonMSTestSpecificTags(tags));
		}

		private IEnumerable<string> GetNonMSTestSpecificTags(IEnumerable<string> tags)
		{
			return tags == null ? new string[0] : tags.Where(t =>
				(!t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase))
				&& (!t.StartsWith(WORKITEM_TAG, StringComparison.InvariantCultureIgnoreCase)))
				.Select(t => t);
		}
	}
}