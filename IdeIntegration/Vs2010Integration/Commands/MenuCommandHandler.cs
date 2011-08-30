using System;
using System.ComponentModel.Design;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public abstract class MenuCommandHandler
    {
        private readonly uint commandId;
        private readonly IServiceProvider serviceProvider;

        public IServiceProvider ServiceProvider
        {
            get { return serviceProvider; }
        }

        protected DTE Dte
        {
            get { return serviceProvider.GetService(typeof(DTE)) as DTE; }
        }


        protected MenuCommandHandler(IServiceProvider serviceProvider, uint commandId)
        {
            this.commandId = commandId;
            this.serviceProvider = serviceProvider;
        }

        public void RegisterTo(OleMenuCommandService menuCommandService)
        {
            CommandID menuCommandID = new CommandID(GuidList.guidSpecFlowCmdSet, (int)commandId);
            OleMenuCommand menuItem = new OleMenuCommand(InvokeHandler, menuCommandID);
            menuItem.BeforeQueryStatus += BeforeQueryStatusHandler;
            menuCommandService.AddCommand(menuItem);
        }

        private void BeforeQueryStatusHandler(object sender, EventArgs eventArgs)
        {
            OleMenuCommand command = sender as OleMenuCommand;
            if (command == null)
                return;

            if (Dte == null)
            {
                command.Visible = false;
                return;
            }

            SelectedItems selection = Dte.SelectedItems;
            if (selection == null)
            {
                command.Visible = false;
                return;
            }
            command.Visible = true;
            command.Enabled = true;

            BeforeQueryStatus(command, selection);
        }

        protected virtual void BeforeQueryStatus(OleMenuCommand command, SelectedItems selection)
        {
            //nop
        }

        private void InvokeHandler(object sender, EventArgs eventArgs)
        {
            OleMenuCommand command = (OleMenuCommand)sender;

            if (Dte == null)
                return;

            SelectedItems selection = Dte.SelectedItems;
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