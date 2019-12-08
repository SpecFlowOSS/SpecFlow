using System.CodeDom;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    class CustomXUnitGeneratorProvider : XUnit2TestGeneratorProvider
    {
        private readonly Combination _combination;

        public CustomXUnitGeneratorProvider(CodeDomHelper codeDomHelper, Combination combination, ProjectSettings projectSettings) : base(codeDomHelper, projectSettings)
        {
            _combination = combination;
        }

        public override void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            base.FinalizeTestClass(generationContext);

            if (_combination != null)
            {
                string programminLanguageEnum = $"TechTalk.SpecFlow.TestProjectGenerator.ProgrammingLanguage.{_combination.ProgrammingLanguage}";
                string projectFormatEnum = $"TechTalk.SpecFlow.TestProjectGenerator.Data.ProjectFormat.{_combination.ProjectFormat}";
                string targetFrameworkEnum = $"TechTalk.SpecFlow.TestProjectGenerator.Data.TargetFramework.{_combination.TargetFramework}";
                string unitTestProviderEnum = $"TechTalk.SpecFlow.TestProjectGenerator.UnitTestProvider.{_combination.UnitTestProvider}";
                string configFormat = $"TechTalk.SpecFlow.TestProjectGenerator.ConfigurationFormat.{_combination.ConfigFormat}";

                generationContext.ScenarioInitializeMethod.Statements.Add(
                    new CodeMethodInvokeExpression(
                        new CodeMethodReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodePropertyReferenceExpression(
                                    new CodeFieldReferenceExpression(null, generationContext.TestRunnerField.Name),
                                    "ScenarioContext"),
                                "ScenarioContainer"),
                            "RegisterInstanceAs",
                            new CodeTypeReference("TechTalk.SpecFlow.TestProjectGenerator.TestRunConfiguration")),
                        new CodeVariableReferenceExpression(
                            $"new TechTalk.SpecFlow.TestProjectGenerator.TestRunConfiguration(){{ ProgrammingLanguage = {programminLanguageEnum}, ProjectFormat = {projectFormatEnum}, TargetFramework = {targetFrameworkEnum}, UnitTestProvider = {unitTestProviderEnum}, ConfigurationFormat = {configFormat} }}")));
            }
        }
    }
}