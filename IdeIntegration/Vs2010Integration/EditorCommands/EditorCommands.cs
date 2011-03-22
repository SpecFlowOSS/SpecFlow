using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using System.Linq;

namespace TechTalk.SpecFlow.Vs2010Integration.EditorCommands
{
    internal class EditorCommands
    {
        private readonly GherkinLanguageService languageService;
        private readonly IWpfTextView textView;

        public EditorCommands(GherkinLanguageService languageService, IWpfTextView textView)
        {
            this.languageService = languageService;
            this.textView = textView;
        }

        private VsStepSuggestionProvider GetSuggestionProvider()
        {
            var suggestionProvider = languageService.ProjectScope.StepSuggestionProvider;
            if (suggestionProvider == null)
                return null;

            return suggestionProvider;
        }

        private GherkinStep GetCurrentStep()
        {
            var fileScope = languageService.GetFileScope();
            if (fileScope == null)
                return null;

            SnapshotPoint caret = textView.Caret.Position.BufferPosition;
            return fileScope.GetStepAtPosition(caret.GetContainingLine().LineNumber);
        }

        public bool CanGoToDefinition()
        {
            return GetSuggestionProvider() != null && GetCurrentStep() != null;
        }

        public bool GoToDefinition()
        {
            var step = GetCurrentStep();
            if (step == null)
                return false;

            var suggestionProvider = GetSuggestionProvider();
            if (suggestionProvider == null)
                return false;

            if (!suggestionProvider.BindingsPopulated)
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.");
                return true;
            }

            var bindings = suggestionProvider.GetMatchingBindings(step).ToArray();
            if (bindings.Length == 0)
            {
                MessageBox.Show("No matching step binding found for this step!");
                return true;
            }
            if (bindings.Length > 1)
            {
                MessageBox.Show("Multiple matching bindings found. Navigating to the first one...");
            }

            var method = bindings[0].StepBinding.Method;
            var codeFunction = new VsStepSuggestionBindingCollector(null).FindCodeFunction(((VsProjectScope) languageService.ProjectScope), method);


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
    }
}