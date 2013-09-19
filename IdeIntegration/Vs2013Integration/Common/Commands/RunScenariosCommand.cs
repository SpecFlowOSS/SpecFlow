using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.EditorCommands;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class RunScenariosCommand : SpecFlowProjectSingleSelectionCommand
    {
        private readonly ITestRunnerEngine testRunnerEngine;

        public RunScenariosCommand(IServiceProvider serviceProvider, DTE dte, ITestRunnerEngine testRunnerEngine) : base(serviceProvider, dte)
        {
            this.testRunnerEngine = testRunnerEngine;
        }

        protected override void BeforeQueryStatus(OleMenuCommand command, SelectedItems selection)
        {
            base.BeforeQueryStatus(command, selection);

            if (command.Visible)
            {
                var selectedItem = selection.Item(1);
                if (selectedItem.ProjectItem != null && VsxHelper.IsPhysicalFile(selectedItem.ProjectItem) && !ContextDependentNavigationCommand.IsFeatureFile(selectedItem.ProjectItem))
                    command.Visible = false; //only show it in feature file, folder or project nodes
            }
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            var selectedItem = selection.Item(1);
            if (selectedItem.ProjectItem != null)
                testRunnerEngine.RunFromProjectItem(selectedItem.ProjectItem, false);
            if (selectedItem.Project != null)
                testRunnerEngine.RunFromProject(selectedItem.Project, false);
        }

        public bool InvokeFromEditor(GherkinEditorContext editorContext, TestRunnerTool? runnerTool)
        {
            return testRunnerEngine.RunFromEditor(editorContext.LanguageService, false, runnerTool);
        }
    }
}
