using System.CodeDom;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.CodeDom;
using TechTalk.SpecFlow.Generator.UnitTestProvider;

namespace TechTalk.SpecFlow.Specs.Generator.SpecFlowPlugin
{
    class CustomXUnitGeneratorProvider : XUnit2TestGeneratorProvider
    {
        private readonly Combination _combination;

        public CustomXUnitGeneratorProvider(CodeDomHelper codeDomHelper, Combination combination) : base(codeDomHelper)
        {
            _combination = combination;
        }

        public override void FinalizeTestClass(TestClassGenerationContext generationContext)
        {
            base.FinalizeTestClass(generationContext);

            generationContext.ScenarioInitializeMethod.Statements.Add(
                new CodeMethodInvokeExpression(
                    new CodeMethodReferenceExpression(
                        new CodePropertyReferenceExpression(
                            new CodePropertyReferenceExpression(
                                new CodeFieldReferenceExpression(null, generationContext.TestRunnerField.Name),
                                "ScenarioContext"),
                            "ScenarioContainer"),
                        "RegisterInstanceAs",
                        new CodeTypeReference("global::SpecFlow.TestProjectGenerator.NewApi.TestRunConfiguration")),
                    new CodeVariableReferenceExpression($"new global::SpecFlow.TestProjectGenerator.NewApi.TestRunConfiguration(global::SpecFlow.TestProjectGenerator.ProgrammingLanguage.{_combination.ProgrammingLanguage}, global::SpecFlow.TestProjectGenerator.NewApi._1_Memory.ProjectFormat.{_combination.ProjectFormat}, global::SpecFlow.TestProjectGenerator.NewApi._1_Memory.TargetFramework.{_combination.TargetFramework})")));
        }
    }
}