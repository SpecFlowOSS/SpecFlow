using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
	public class XUnitTestGeneratorProvider : IUnitTestGeneratorProvider
	{
		private const string FEATURE_TITLE_KEY = "FeatureTitle";
		private const string FEATURE_TITLE_PROPERTY_NAME = "FeatureTitle";
		private const string FACT_ATTRIBUTE = "Xunit.FactAttribute";
		private const string FACT_ATTRIBUTE_SKIP_PROPERTY_NAME = "Skip";
		private const string SKIP_REASON = "Ignored";
		private const string TRAIT_ATTRIBUTE = "Xunit.TraitAttribute";
		private const string IUSEFIXTURE = "Xunit.IUseFixture";	// n/a uses IUseFixture<T>, what to do?
		private const string TESTSETUP_ATTR = "Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute"; // n/a uses the Constructor, what to do?
		private const string IDISPOSABLE_METHOD_NAME = "Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute";	// n/a uses IDisposable.Dispose, what to do?

		private CodeTypeDeclaration _currentTypeDeclaration = null;

		public void SetTestFixture(CodeTypeDeclaration typeDeclaration, string title, string description)
		{
			// xUnit does not use an attribute for the TestFixture

			// Remember the feature title for use later
			typeDeclaration.UserData[FEATURE_TITLE_KEY] = title;

			_currentTypeDeclaration = typeDeclaration;
		}

		public void SetTestFixtureCategories(CodeTypeDeclaration typeDeclaration, IEnumerable<string> categories)
		{
			// xUnit does not support caregories
		}

		public void SetTestFixtureSetup(CodeMemberMethod memberMethod)
		{
			// xUnit uses IUseFixture<T> on the class
		}

		public void SetTestFixtureTearDown(CodeMemberMethod memberMethod)
		{
			// xUnit uses IUseFixture<T> on the class
		}

		public void SetTest(CodeMemberMethod memberMethod, string title)
		{
			memberMethod.CustomAttributes.Add
			(
				new CodeAttributeDeclaration
				(
					new CodeTypeReference(FACT_ATTRIBUTE)
				)
			);

			if (_currentTypeDeclaration != null)
			{
				string featureTitle = _currentTypeDeclaration.UserData[FEATURE_TITLE_KEY] as string;
				if (!string.IsNullOrEmpty(featureTitle))
				{
					SetProperty(memberMethod.CustomAttributes, FEATURE_TITLE_PROPERTY_NAME, featureTitle);
				}
			}

			SetDescription(memberMethod.CustomAttributes, title);
		}

		public void SetTestCategories(CodeMemberMethod memberMethod, IEnumerable<string> categories)
		{
			// xUnit does not support caregories
		}

		public void SetTestSetup(CodeMemberMethod memberMethod)
		{
			// xUnit uses a parameterless Constructor
		}

		public void SetTestTearDown(CodeMemberMethod memberMethod)
		{
			// uses IDisposable.Dispose, what to do?
		}

		public void SetIgnore(CodeTypeMember codeTypeMember)
		{
			foreach (var customAttribute in codeTypeMember.CustomAttributes)
			{
				CodeAttributeDeclaration codeAttributeDeclaration = customAttribute as CodeAttributeDeclaration;
				if (codeAttributeDeclaration != null && codeAttributeDeclaration.Name == FACT_ATTRIBUTE)
				{
					// set [FactAttribute(Skip="reason")]
					codeAttributeDeclaration.Arguments.Add
					(
						new CodeAttributeArgument(FACT_ATTRIBUTE_SKIP_PROPERTY_NAME, new CodePrimitiveExpression(SKIP_REASON))
					);
					break;
				}
			}
		}

		private void SetProperty(CodeAttributeDeclarationCollection customAttributes, string name, string value)
		{
			customAttributes.Add
			(
				new CodeAttributeDeclaration
				(
					new CodeTypeReference(TRAIT_ATTRIBUTE),
					new CodeAttributeArgument
					(
						new CodePrimitiveExpression(name)
					),
					new CodeAttributeArgument
					(
						new CodePrimitiveExpression(value)
					)
				)
			);
		}

		private void SetDescription(CodeAttributeDeclarationCollection customAttributes, string description)
		{
			// xUnit doesn't have a DescriptionAttribute so using a TraitAttribute instead
			customAttributes.Add
			(
				new CodeAttributeDeclaration
				(
					new CodeTypeReference(TRAIT_ATTRIBUTE),
					new CodeAttributeArgument
					(
						new CodePrimitiveExpression("Description")
					),
					new CodeAttributeArgument
					(
						new CodePrimitiveExpression(description)
					)
				)
			);
		}
	}
}