using System;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class ReGenerateAllCommand : SpecFlowProjectSingleSelectionCommand
    {
        private readonly IProjectScopeFactory projectScopeFactory;

        public ReGenerateAllCommand(IServiceProvider serviceProvider, DTE dte, IProjectScopeFactory projectScopeFactory)
            : base(serviceProvider, dte)
        {
            this.projectScopeFactory = projectScopeFactory;
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            var selectedItem = selection.Item(1);
            if (selectedItem.Project == null)
                return;

            var projectScope = projectScopeFactory.GetProjectScope(selectedItem.Project) as VsProjectScope;
            if (projectScope == null)
                return;
            
            if (!projectScope.FeatureFilesTracker.IsInitialized)
            {
                MessageBox.Show("Feature files are still being analyzed. Please wait.", "Regenerate Feature Files");
                return;
            }

            projectScope.FeatureFilesTracker.ReGenerateAll();
        }
    }
}
