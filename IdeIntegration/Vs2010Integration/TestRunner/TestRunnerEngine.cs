using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public interface ITestRunnerEngine
    {
        bool RunFromEditor(GherkinLanguageService languageService, bool debug);
        bool RunFromProjectItem(ProjectItem projectItem, bool debug);
        bool RunFromProject(Project project, bool debug);
    }

    public class TestRunnerEngine : ITestRunnerEngine
    {
        private readonly DTE dte;
        private readonly ITestRunnerGatewayProvider testRunnerGatewayProvider;

        public TestRunnerEngine(DTE dte, ITestRunnerGatewayProvider testRunnerGatewayProvider)
        {
            this.dte = dte;
            this.testRunnerGatewayProvider = testRunnerGatewayProvider;
        }

        public bool RunFromEditor(GherkinLanguageService languageService, bool debug)
        {
            if (dte.ActiveDocument == null || dte.ActiveDocument.ProjectItem == null)
                return false;

            IGherkinFileScope fileScope;
            var currentScenario = GetCurrentScenario(languageService, dte.ActiveDocument, out fileScope);
            if (currentScenario == null)
            {
                // run for the entire file
                return RunFromProjectItem(dte.ActiveDocument.ProjectItem, debug);
            }

            var testRunnerGateway = testRunnerGatewayProvider.GetTestRunnerGateway();
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunScenario(dte.ActiveDocument.ProjectItem, currentScenario, fileScope, debug);
        }

        public bool RunFromProjectItem(ProjectItem projectItem, bool debug)
        {
            var testRunnerGateway = testRunnerGatewayProvider.GetTestRunnerGateway();
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(projectItem, debug);
        }

        public bool RunFromProject(Project project, bool debug)
        {
            var testRunnerGateway = testRunnerGatewayProvider.GetTestRunnerGateway();
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(project, debug);
        }

        private IScenarioBlock GetCurrentScenario(GherkinLanguageService languageService, Document activeDocument, out IGherkinFileScope fileScope)
        {
            var currentTextDocument = ((TextDocument)activeDocument.Object("TextDocument"));
            var currentLine = currentTextDocument.Selection.ActivePoint.Line;

            fileScope = languageService.GetFileScope();
            if (fileScope == null)
                return null;

            var block = fileScope.GetStepBlockFromStepPosition(currentLine) as IScenarioBlock;
            if (block != null && currentLine > block.KeywordLine + block.BlockRelativeContentEndLine)
                return null; // between two scenarios

            return block;
        }
    }
}