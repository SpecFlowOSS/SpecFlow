using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EnvDTE;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class ContextDependentNavigationCommand : MenuCommandHandler
    {
        private readonly GoToStepsCommand goToStepsCommand;

        public ContextDependentNavigationCommand(IServiceProvider serviceProvider, DTE dte, GoToStepsCommand goToStepsCommand) : base(serviceProvider, dte)
        {
            this.goToStepsCommand = goToStepsCommand;
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
                //TODO
                command.Enabled = true;
                command.Text = "Go To Step Definition";
            }
            else if (IsCodeFile(activeDocument.ProjectItem))
            {
                command.Enabled = goToStepsCommand.IsStepDefinition(activeDocument);
                command.Text = "Go To Steps";
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
                throw new NotImplementedException(); //TODO
            }
            else if (IsCodeFile(activeDocument.ProjectItem))
            {
                goToStepsCommand.Invoke(activeDocument);
            }
        }

        private bool IsFeatureFile(ProjectItem projectItem)
        {
            return projectItem.Name.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase);
        }

        private bool IsCodeFile(ProjectItem projectItem)
        {
            return projectItem.FileCodeModel != null;
        }
    }
}
