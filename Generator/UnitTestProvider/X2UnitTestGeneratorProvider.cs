using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public class XUnit2TestGeneratorProvider : XUnitTestGeneratorProvider
    {
        private const string FEATURE_TITLE_PROPERTY_NAME = "FeatureTitle";
        private const string THEORY_ATTRIBUTE = "Xunit.TheoryAttribute";
        private const string INLINEDATA_ATTRIBUTE = "Xunit.InlineDataAttribute";
        private const string ICOLLECTIONFIXTURE_INTERFACE = "Xunit.ICollectionFixture";

        public XUnit2TestGeneratorProvider(CodeDomHelper codeDomHelper)
            :base(codeDomHelper)
        {
            CodeDomHelper = codeDomHelper;
        }

        protected override CodeTypeReference CreateFixtureInterface(CodeTypeReference fixtureDataType)
        {
            return new CodeTypeReference(ICOLLECTIONFIXTURE_INTERFACE, fixtureDataType);
        }

        public override void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle)
        {
            CodeDomHelper.AddAttribute(testMethod, THEORY_ATTRIBUTE);

            SetProperty(testMethod, FEATURE_TITLE_PROPERTY_NAME, generationContext.Feature.Title);
            SetDescription(testMethod, scenarioTitle);
        }

        public override void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments,
            IEnumerable<string> tags, bool isIgnored)
        {
            //TODO: better handle "ignored"
            if (isIgnored)
                return;

            var args = arguments.Select(
              arg => new CodeAttributeArgument(new CodePrimitiveExpression(arg))).ToList();

            args.Add(
                new CodeAttributeArgument(
                    new CodeArrayCreateExpression(typeof(string[]), tags.Select(t => new CodePrimitiveExpression(t)).ToArray())));

            CodeDomHelper.AddAttribute(testMethod, INLINEDATA_ATTRIBUTE, args.ToArray());
        }
    }
}