using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public interface IUnitTestGeneratorProvider
    {
        bool SupportsRowTests { get; }
        bool SupportsAsyncTests { get; }

        void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription);
        void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories);
        void SetTestClassIgnore(TestClassGenerationContext generationContext);
        void FinalizeTestClass(TestClassGenerationContext generationContext);

        void SetTestClassInitializeMethod(TestClassGenerationContext generationContext);
        void SetTestClassCleanupMethod(TestClassGenerationContext generationContext);

        void SetTestInitializeMethod(TestClassGenerationContext generationContext); 
        void SetTestCleanupMethod(TestClassGenerationContext generationContext);

        void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle);
        void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories);
        void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod);

        void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle);
        void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored);
        void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments);
    }
}