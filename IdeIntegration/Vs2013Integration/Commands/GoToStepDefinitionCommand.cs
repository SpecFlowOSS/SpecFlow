using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Vs2010Integration.Bindings.Discovery;
using TechTalk.SpecFlow.Vs2010Integration.EditorCommands;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;

namespace TechTalk.SpecFlow.Vs2010Integration.Commands
{
    public class GoToStepDefinitionCommand : SpecFlowProjectSingleSelectionCommand
    {
        private readonly IGherkinLanguageServiceFactory gherkinLanguageServiceFactory;
        private readonly IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider;

        public GoToStepDefinitionCommand(IServiceProvider serviceProvider, DTE dte, IGherkinLanguageServiceFactory gherkinLanguageServiceFactory, IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider) : base(serviceProvider, dte)
        {
            this.gherkinLanguageServiceFactory = gherkinLanguageServiceFactory;
            this.stepDefinitionSkeletonProvider = stepDefinitionSkeletonProvider;
        }

        internal bool IsEnabled(Document activeDocument)
        {
            var editorContext = GherkinEditorContext.FromDocument(activeDocument, gherkinLanguageServiceFactory);
            if (editorContext == null) 
                return false;

            return CanGoToDefinition(editorContext);
        }

        internal void Invoke(Document activeDocument)
        {
            var editorContext = GherkinEditorContext.FromDocument(activeDocument, gherkinLanguageServiceFactory);
            if (editorContext == null) 
                return;

            GoToDefinition(editorContext);
        }

        public bool CanGoToDefinition(GherkinEditorContext editorContext)
        {
            return GetBindingMatchService(editorContext.LanguageService) != null && GetCurrentStep(editorContext) != null;
        }

        public bool GoToDefinition(GherkinEditorContext editorContext)
        {
            var step = GetCurrentStep(editorContext);
            if (step == null)
                return false;

            var bindingMatchService = GetBindingMatchService(editorContext.LanguageService);
            if (bindingMatchService == null)
                return false;

            if (!bindingMatchService.Ready)
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to binding");
                return true;
            }

            List<BindingMatch> candidatingMatches;
            StepDefinitionAmbiguityReason ambiguityReason;
            CultureInfo bindingCulture = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.BindingCulture ?? step.StepContext.Language;
            var match = bindingMatchService.GetBestMatch(step, bindingCulture, out ambiguityReason, out candidatingMatches);
            var binding = match.StepBinding;

            if (!match.Success)
            {
                if (candidatingMatches.Any())
                {
                    string bindingsText = string.Join(Environment.NewLine, candidatingMatches.Select(b => b.StepBinding.Method.GetShortDisplayText()));
                    MessageBox.Show("Multiple matching bindings found. Navigating to the first match..."
                        + Environment.NewLine + Environment.NewLine + bindingsText, "Go to binding");
                    binding = candidatingMatches.First().StepBinding;
                }
                else
                {
                    var language = editorContext.ProjectScope is VsProjectScope ? VsProjectScope.GetTargetLanguage(((VsProjectScope) editorContext.ProjectScope).Project) : ProgrammingLanguage.CSharp;
                    var stepDefinitionSkeletonStyle = editorContext.ProjectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.StepDefinitionSkeletonStyle;
                    string skeleton = stepDefinitionSkeletonProvider.GetStepDefinitionSkeleton(language, step, stepDefinitionSkeletonStyle, bindingCulture);

                    var result = MessageBox.Show("No matching step binding found for this step! Do you want to copy the step binding skeleton to the clipboard?"
                         + Environment.NewLine + Environment.NewLine + skeleton, "Go to binding", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        Clipboard.SetText(skeleton);
                    }
                    return true;
                }
            }

            var method = binding.Method;
            var codeFunction = new VsBindingMethodLocator().FindCodeFunction(((VsProjectScope) editorContext.ProjectScope), method);

            if (codeFunction != null)
            {
                if (!codeFunction.ProjectItem.IsOpen)
                {
                    codeFunction.ProjectItem.Open();
                }
                var navigatePoint = codeFunction.GetStartPoint(vsCMPart.vsCMPartHeader);
                navigatePoint.TryToShow();
                navigatePoint.Parent.Selection.MoveToPoint(navigatePoint);
            }

            return true;
        }

        private IStepDefinitionMatchService GetBindingMatchService(GherkinLanguageService languageService)
        {
            var bindingMatchService = languageService.ProjectScope.BindingMatchService;
            if (bindingMatchService == null)
                return null;

            return bindingMatchService;
        }

        private GherkinStep GetCurrentStep(GherkinEditorContext editorContext)
        {
            var fileScope = editorContext.LanguageService.GetFileScope();
            if (fileScope == null)
                return null;

            SnapshotPoint caret = editorContext.TextView.Caret.Position.BufferPosition;
            IStepBlock block;
            var step = fileScope.GetStepAtPosition(caret.GetContainingLine().LineNumber, out block);

            if (step != null && block is IScenarioOutlineBlock)
                step = step.GetSubstitutedStep((IScenarioOutlineBlock)block);

            return step;
        }
    }
}
