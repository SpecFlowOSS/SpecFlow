using System;
using EnvDTE;
using Microsoft.VisualStudio.Shell;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class DelegateMenuCommandHandler : MenuCommandHandler
    {
        private readonly Action<OleMenuCommand, SelectedItems> invoke;
        private readonly Action<OleMenuCommand, SelectedItems> beforeQueryStatus;

        public DelegateMenuCommandHandler(IServiceProvider serviceProvider, DTE dte, 
                                          Action<OleMenuCommand, SelectedItems> invoke, Action<OleMenuCommand, SelectedItems> beforeQueryStatus = null) 
            : base(serviceProvider, dte)
        {
            this.invoke = invoke;
            this.beforeQueryStatus = beforeQueryStatus ??
                                     ((command, _) =>
                                          {
                                              command.Visible = true;
                                              command.Enabled = true;
                                          });
        }

        protected override void BeforeQueryStatus(OleMenuCommand command, SelectedItems selection)
        {
            if (beforeQueryStatus != null)
                beforeQueryStatus(command, selection);
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            invoke(command, selection);
        }
    }
}