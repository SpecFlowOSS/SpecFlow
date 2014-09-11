using System;
using System.Linq;
using EnvDTE;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class ContextDependentNavigationCommand : MenuCommandHandler
    {
        private readonly IGoToStepsCommand goToStepsCommand;
        private readonly IGoToStepDefinitionCommand goToStepDefinitionCommand;

        public ContextDependentNavigationCommand(IServiceProvider serviceProvider, DTE dte, IGoToStepsCommand goToStepsCommand, IGoToStepDefinitionCommand goToStepDefinitionCommand) : base(serviceProvider, dte)
        {
            this.goToStepsCommand = goToStepsCommand;
            this.goToStepDefinitionCommand = goToStepDefinitionCommand;
        }

        public bool IsCodeFile(ProjectItem projectItem)
        {
            return VsProjectScope.IsCodeFileSupported(projectItem);
        }

        protected override void BeforeQueryStatus(Microsoft.VisualStudio.Shell.OleMenuCommand command, SelectedItems selection)
        {
            var activeDocument = dte.ActiveDocument;
            if (activeDocument == null || activeDocument.ProjectItem == null)
            {
                command.Visible = false;
                return;
            }

            if (selection.Count != 1)
            {
                command.Enabled = false;
                return;
            }

            if (IsFeatureFile(activeDocument.ProjectItem))
            {
                command.Enabled = goToStepDefinitionCommand.IsEnabled(activeDocument);
                command.Text = "Go To Step Definition";
            }
            else if (IsCodeFile(activeDocument.ProjectItem))
            {
                command.Enabled = goToStepsCommand.IsEnabled(activeDocument);
                command.Text = "Go To SpecFlow Step Definition Usages";
            }
            else
            {
                command.Visible = false;
            }
        }

        protected override void Invoke(Microsoft.VisualStudio.Shell.OleMenuCommand command, SelectedItems selection)
        {
            var activeDocument = dte.ActiveDocument;

            if (IsFeatureFile(activeDocument.ProjectItem))
            {
                goToStepDefinitionCommand.Invoke(activeDocument);
            }
            else if (IsCodeFile(activeDocument.ProjectItem))
            {
                goToStepsCommand.Invoke(activeDocument);
            }
        }

        internal static bool IsFeatureFile(ProjectItem projectItem)
        {
            return projectItem.Name.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
