using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
	public class MsTest2010GeneratorProvider : MsTestGeneratorProvider
	{
		private const string CATEGORY_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute";
		private const string OWNER_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.OwnerAttribute";

		private const string OWNER_TAG = "owner=";

		public override void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories)
		{
			generationContext.CustomData["featureCategories"] = GetNonMSTestSpecificTags(featureCategories).ToArray();

			IEnumerable<string> ownerTags = featureCategories.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
			if(ownerTags.Count() > 0)
			{
				generationContext.CustomData[OWNER_TAG] = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
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
		}

		public override void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories)
		{
			IEnumerable<string> tags = scenarioCategories.ToList();

			IEnumerable<string> ownerTags = tags.Where(t => t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)).Select(t => t);
			if(ownerTags.Count() > 0)
			{
				string ownerName = ownerTags.Select(t => t.Substring(OWNER_TAG.Length).Trim('\"')).FirstOrDefault();
				if(!String.IsNullOrEmpty(ownerName))
				{
					CodeDomHelper.AddAttribute(testMethod, OWNER_ATTR, ownerName);
				}
			}

			CodeDomHelper.AddAttributeForEachValue(testMethod, CATEGORY_ATTR, GetNonMSTestSpecificTags(tags));
		}

		private IEnumerable<string> GetNonMSTestSpecificTags(IEnumerable<string> tags)
		{
			return tags == null ? new string[0] : tags.Where(t =>
				(!t.StartsWith(OWNER_TAG, StringComparison.InvariantCultureIgnoreCase)))
				.Select(t => t);
		}
	}
}