using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Vs2010Integration.EditorCommands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.UI;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class GenerateStepDefinitionSkeletonCommand  : SpecFlowProjectSingleSelectionCommand
    {
        private readonly IGherkinLanguageServiceFactory gherkinLanguageServiceFactory;

        public GenerateStepDefinitionSkeletonCommand(IServiceProvider serviceProvider, DTE dte, IGherkinLanguageServiceFactory gherkinLanguageServiceFactory) : base(serviceProvider, dte)
        {
            this.gherkinLanguageServiceFactory = gherkinLanguageServiceFactory;
        }

        protected override void Invoke(OleMenuCommand command, SelectedItems selection)
        {
            var activeDocument = dte.ActiveDocument;
            Invoke(activeDocument);
        }

        private void Invoke(Document activeDocument)
        {
            var editorContext = GherkinEditorContext.FromDocument(activeDocument, gherkinLanguageServiceFactory);
            if (editorContext == null) 
                return;

            GenerateStepDefinitionSkeleton(editorContext);
        }

        private bool GenerateStepDefinitionSkeleton(GherkinEditorContext editorContext)
        {
            var bindingMatchService = GetBindingMatchService(editorContext.LanguageService);
            if (bindingMatchService == null)
                return false;

            var fileScope = editorContext.LanguageService.GetFileScope(waitForLatest: true);
            if (fileScope == null)
                return false;

            var featureTitle = GetFeatureTitle(fileScope);
            var steps = GetUnboundSteps(bindingMatchService, editorContext, fileScope).ToArray();

            if (steps.Length == 0)
            {
                MessageBox.Show("All the steps are bound!", "Generate step definition skeleton");
                return true;
            }

            using (var form = new GenerateStepDefinitionSkeletonForm(featureTitle, steps))
            {
                var result = form.ShowDialog();
                if (result == DialogResult.OK)
                {
                    MessageBox.Show("TODO");
                }
            }

            return true;
        }

        private string GetFeatureTitle(IGherkinFileScope fileScope)
        {
            if (fileScope.HeaderBlock == null || string.IsNullOrWhiteSpace(fileScope.HeaderBlock.Title))
                return "Unknown";

            return fileScope.HeaderBlock.Title;
        }

        private IEnumerable<StepInstance> GetUnboundSteps(IStepDefinitionMatchService bindingMatchService, GherkinEditorContext editorContext, IGherkinFileScope fileScope)
        {
            CultureInfo bindingCulture = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.BindingCulture ?? fileScope.GherkinDialect.CultureInfo;
            return fileScope.GetAllSteps().Where(s => !IsBound(s, bindingMatchService, bindingCulture));
        }

        private static bool IsBound(GherkinStep step, IStepDefinitionMatchService bindingMatchService, CultureInfo bindingCulture)
        {
            List<BindingMatch> candidatingMatches;
            StepDefinitionAmbiguityReason ambiguityReason;
            var match = bindingMatchService.GetBestMatch(step, bindingCulture, out ambiguityReason, out candidatingMatches);
            bool isBound = match.Success;
            return isBound;
        }

        private IStepDefinitionMatchService GetBindingMatchService(GherkinLanguageService languageService)
        {
            var bindingMatchService = languageService.ProjectScope.BindingMatchService;
            if (bindingMatchService == null)
                return null;

            if (!bindingMatchService.Ready)
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Generate step definition skeleton");
                return null;
            }

            return bindingMatchService;
        }
    }
}
