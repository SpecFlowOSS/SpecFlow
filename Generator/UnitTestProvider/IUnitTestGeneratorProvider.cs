using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public interface IUnitTestGeneratorProvider
    {
        bool SupportsRowTests { get; }
        bool SupportsAsyncTests { get; }

        void SetTestFixture(TestClassGenerationContext generationContext, string featureTitle, string featureDescription);
        void SetTestFixtureCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories);
        void SetTestClassIgnore(TestClassGenerationContext generationContext);
        void FinalizeTestClass(TestClassGenerationContext generationContext);

        void SetTestFixtureSetup(TestClassGenerationContext generationContext);
        void SetTestFixtureTearDown(TestClassGenerationContext generationContext);

        void SetTestSetup(TestClassGenerationContext generationContext); 
        void SetTestTearDown(TestClassGenerationContext generationContext);

        void SetTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle);
        void SetTestCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories);
        void SetIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod);

        void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle);
        void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored);
        void SetTestVariant(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments);
    }
}