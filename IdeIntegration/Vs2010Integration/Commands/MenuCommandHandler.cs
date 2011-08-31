using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public abstract class MenuCommandHandler
    {
        protected readonly IServiceProvider serviceProvider;
        protected readonly DTE dte;

        protected MenuCommandHandler(IServiceProvider serviceProvider, DTE dte)
        {
            this.serviceProvider = serviceProvider;
            this.dte = dte;
        }

        public void RegisterTo(OleMenuCommandService menuCommandService, SpecFlowCmdSet commandId)
        {
            CommandID menuCommandID = new CommandID(GuidList.guidSpecFlowCmdSet, (int)(uint)commandId);
            OleMenuCommand menuItem = new OleMenuCommand(InvokeHandler, menuCommandID);
            menuItem.BeforeQueryStatus += BeforeQueryStatusHandler;
            menuCommandService.AddCommand(menuItem);
        }

        private void BeforeQueryStatusHandler(object sender, EventArgs eventArgs)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command == null)
                return;

            if (dte == null)
            {
                command.Visible = false;
                return;
            }

            SelectedItems selection = dte.SelectedItems;
            if (selection == null)
            {
                command.Visible = false;
                return;
            }
            command.Visible = true;
            command.Enabled = false;

            BeforeQueryStatus(command, selection);
        }

        protected virtual void BeforeQueryStatus(OleMenuCommand command, SelectedItems selection)
        {
            //nop
        }

        private void InvokeHandler(object sender, EventArgs eventArgs)
        {
            OleMenuCommand command = (OleMenuCommand)sender;

            if (dte == null)
                return;

            SelectedItems selection = dte.SelectedItems;
            if (selection == null)
                return;

            Invoke(command, selection);
        }

        protected virtual void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            System.Windows.MessageBox.Show("Command executed");
        }
    }
}