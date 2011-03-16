using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.AutoComplete;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;
using System.Linq;
using ScenarioBlock = TechTalk.SpecFlow.Parser.Gherkin.ScenarioBlock;

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

        public bool GoToDefinition()
        {
            var suggestionProvider = languageService.ProjectScope.StepSuggestionProvider;
            if (suggestionProvider == null)
                return false;

            if (!suggestionProvider.Populated)
                return false;



            SnapshotPoint caret = textView.Caret.Position.BufferPosition;

            if (!GherkinStepCompletionSource.IsStepLine(caret, languageService))
                return false;

            var line = caret.GetContainingLine().GetText().Trim();
            var parts = line.Split(new[]{' '}, 2);
            if (parts.Length != 2)
                return false;

            var stepInstance = new StepInstance<Completion>(new ScenarioStep() { Text = parts[1], 
                ScenarioBlock = parts[0] == "When" ? ScenarioBlock.When : parts[0] == "Then" ? ScenarioBlock.Then : ScenarioBlock.Given}, new Scenario(), new Feature(), VsSuggestionItemFactory.Instance);
            var bindings = suggestionProvider.GetMatchingBindings(stepInstance).ToArray();
            if (bindings.Length == 0)
            {
                MessageBox.Show("not bound?");
                return true;
            }
            if (bindings.Length > 1)
            {
                MessageBox.Show("multiple bindings?");
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