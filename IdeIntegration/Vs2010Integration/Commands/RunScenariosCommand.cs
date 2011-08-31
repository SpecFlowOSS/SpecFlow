using System;
using System.Linq;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class RunScenariosCommand : MenuCommandHandler
    {
        private readonly ITestRunnerEngine testRunnerEngine;

        public RunScenariosCommand(IServiceProvider serviceProvider, DTE dte, ITestRunnerEngine testRunnerEngine) : base(serviceProvider, dte)
        {
            this.testRunnerEngine = testRunnerEngine;
        }

        protected override void BeforeQueryStatus(OleMenuCommand command, SelectedItems selection)
        {
            if (!IsInSpecFlowProject(selection))
            {
                command.Visible = false;
                return;
            }

            if (selection.Count == 1)
            {
                command.Enabled = true;
            }
        }

        private bool IsInSpecFlowProject(SelectedItems selection)
        {
            for (int selectionIndex = 1; selectionIndex <= selection.Count; selectionIndex++)
                if (IsInSpecFlowProject(selection.Item(selectionIndex)))
                    return true;
            return false;
        }

        private bool IsInSpecFlowProject(SelectedItem selectedItem)
        {
            if (selectedItem.Project != null)
                return IsInSpecFlowProject(selectedItem.Project);
            if (selectedItem.ProjectItem != null)
                return IsInSpecFlowProject(selectedItem.ProjectItem.ContainingProject);
            return false;
        }

        private bool IsInSpecFlowProject(Project project)
        {
            return VsxHelper.GetReference(project, "TechTalk.SpecFlow") != null;
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            var selectedItem = selection.Item(1);
            if (selectedItem.ProjectItem != null)
                testRunnerEngine.RunFromProjectItem(selectedItem.ProjectItem);
            if (selectedItem.Project != null)
                testRunnerEngine.RunFromProject(selectedItem.Project);
        }
    }
}
