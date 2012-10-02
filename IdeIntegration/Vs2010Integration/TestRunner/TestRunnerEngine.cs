using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public interface ITestRunnerEngine
    {
        bool RunFromEditor(GherkinLanguageService languageService, bool debug, TestRunnerTool? runnerTool = null);
        bool RunFromProjectItem(ProjectItem projectItem, bool debug, TestRunnerTool? runnerTool = null);
        bool RunFromProject(Project project, bool debug, TestRunnerTool? runnerTool = null);
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

        public bool RunFromEditor(GherkinLanguageService languageService, bool debug, TestRunnerTool? runnerTool = null)
        {
            if (dte.ActiveDocument == null || dte.ActiveDocument.ProjectItem == null)
                return false;

            IGherkinFileScope fileScope;
            int currentLine = GetCurrentLine(dte.ActiveDocument);
            var currentScenario = GetCurrentScenario(languageService, currentLine, out fileScope);
            if (currentScenario == null)
            {
                // run for the entire file
                return RunFromProjectItem(dte.ActiveDocument.ProjectItem, debug, runnerTool);
            }

            //TODO: select the scenario outline example, if it was invoked from that line

            var testRunnerGateway = testRunnerGatewayProvider.GetTestRunnerGateway(runnerTool);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunScenario(dte.ActiveDocument.ProjectItem, currentScenario, fileScope, debug);
        }

        public bool RunFromProjectItem(ProjectItem projectItem, bool debug, TestRunnerTool? runnerTool = null)
        {
            var testRunnerGateway = testRunnerGatewayProvider.GetTestRunnerGateway(runnerTool);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(projectItem, debug);
        }

        public bool RunFromProject(Project project, bool debug, TestRunnerTool? runnerTool = null)
        {
            var testRunnerGateway = testRunnerGatewayProvider.GetTestRunnerGateway(runnerTool);
            if (testRunnerGateway == null)
                return false;

            return testRunnerGateway.RunFeatures(project, debug);
        }

        private IScenarioBlock GetCurrentScenario(GherkinLanguageService languageService, int currentLine, out IGherkinFileScope fileScope)
        {
            fileScope = languageService.GetFileScope();
            if (fileScope == null)
                return null;

            var block = fileScope.GetStepBlockFromStepPosition(currentLine) as IScenarioBlock;
            if (block != null && currentLine > block.KeywordLine + block.BlockRelativeContentEndLine)
                return null; // between two scenarios

            return block;
        }

        private int GetCurrentLine(Document activeDocument)
        {
            var currentTextDocument = ((TextDocument) activeDocument.Object("TextDocument"));
            var currentLine = currentTextDocument.Selection.ActivePoint.Line;
            return currentLine;
        }
    }
}