using System.CodeDom;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Generator.UnitTestProvider
{
    public interface IUnitTestGeneratorProvider
    {
        UnitTestGeneratorTraits GetTraits();

        void SetTestClass(TestClassGenerationContext generationContext, string featureTitle, string featureDescription);
        void SetTestClassCategories(TestClassGenerationContext generationContext, IEnumerable<string> featureCategories);
        void SetTestClassIgnore(TestClassGenerationContext generationContext);
        void FinalizeTestClass(TestClassGenerationContext generationContext);
        void SetTestClassNonParallelizable(TestClassGenerationContext generationContext);

        void SetTestClassInitializeMethod(TestClassGenerationContext generationContext);
        void SetTestClassCleanupMethod(TestClassGenerationContext generationContext);

        void SetTestInitializeMethod(TestClassGenerationContext generationContext);
        void SetTestCleanupMethod(TestClassGenerationContext generationContext);

        void SetTestMethod(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string friendlyTestName);
        void SetTestMethodCategories(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> scenarioCategories);
        void SetTestMethodIgnore(TestClassGenerationContext generationContext, CodeMemberMethod testMethod);

        void SetRowTest(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle);
        void SetRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, IEnumerable<string> arguments, IEnumerable<string> tags, bool isIgnored);
        void SetTestMethodAsRow(TestClassGenerationContext generationContext, CodeMemberMethod testMethod, string scenarioTitle, string exampleSetName, string variantName, IEnumerable<KeyValuePair<string, string>> arguments);

        void MarkCodeMethodInvokeExpressionAsAwait(CodeMethodInvokeExpression expression);

        CodeExpression GetTestWorkerIdExpression();
    }
}
