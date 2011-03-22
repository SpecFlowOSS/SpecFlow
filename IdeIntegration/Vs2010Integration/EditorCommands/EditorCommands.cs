using System;
using System.Collections.Generic;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;
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

        private IBindingMatchService GetBindingMatchService()
        {
            var bindingMatchService = languageService.ProjectScope.BindingMatchService;
            if (bindingMatchService == null)
                return null;

            return bindingMatchService;
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
            return GetBindingMatchService() != null && GetCurrentStep() != null;
        }

        public bool GoToDefinition()
        {
            var step = GetCurrentStep();
            if (step == null)
                return false;

            var bindingMatchService = GetBindingMatchService();
            if (bindingMatchService == null)
                return false;

            if (!bindingMatchService.Ready)
            {
                MessageBox.Show("Step bindings are still being analyzed. Please wait.", "Go to binding");
                return true;
            }

            IEnumerable<StepBinding> candidatingBindings;
            var binding = bindingMatchService.GetBestMatchingBinding(step, out candidatingBindings);

            if (binding == null)
            {
                if (candidatingBindings.Any())
                {
                    string bindingsText = string.Join(Environment.NewLine, candidatingBindings.Select(b => b.Method.ShortDisplayText));
                    MessageBox.Show("Multiple matching bindings found. Navigating to the first match..."
                        + Environment.NewLine + Environment.NewLine + bindingsText, "Go to binding");
                    binding = candidatingBindings.First();
                }
                else
                {
                    MessageBox.Show("No matching step binding found for this step!", "Go to binding");
                    return true;
                }
            }

            var method = binding.Method;
            var codeFunction = new VsStepSuggestionBindingCollector().FindCodeFunction(((VsProjectScope) languageService.ProjectScope), method);

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