using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class DebugScenariosCommand : SpecFlowProjectSingleSelectionCommand
    {
        private readonly ITestRunnerEngine testRunnerEngine;

        public DebugScenariosCommand(IServiceProvider serviceProvider, DTE dte, ITestRunnerEngine testRunnerEngine) : base(serviceProvider, dte)
        {
            this.testRunnerEngine = testRunnerEngine;
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            var selectedItem = selection.Item(1);
            if (selectedItem.ProjectItem != null)
                testRunnerEngine.RunFromProjectItem(selectedItem.ProjectItem, true);
            if (selectedItem.Project != null)
                testRunnerEngine.RunFromProject(selectedItem.Project, true);
        }
    }
}