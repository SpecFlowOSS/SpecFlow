using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public abstract class SpecFlowProjectSingleSelectionCommand : MenuCommandHandler
    {
        protected SpecFlowProjectSingleSelectionCommand(IServiceProvider serviceProvider, DTE dte) : base(serviceProvider, dte)
        {
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
    }
}