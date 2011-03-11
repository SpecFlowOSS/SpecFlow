using System.Collections.Generic;
using System.Text;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public class StepInstance<TNativeSuggestionItem> : IBoundStepSuggestion<TNativeSuggestionItem>
    {
        private readonly List<BoundStepSuggestions<TNativeSuggestionItem>> matchGroups = new List<BoundStepSuggestions<TNativeSuggestionItem>>(1);
        public ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get { return matchGroups; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }
        public StepInstanceTemplate<TNativeSuggestionItem> ParentTemplate { get; internal set; }

        public BindingType BindingType { get { return (BindingType)step.ScenarioBlock; } }
        public string StepText { get { return step.Text; } }

        private readonly ScenarioStep step;
        private readonly Scenario scenario;
        private readonly Feature feature;

        public StepInstance(ScenarioStep step, Scenario scenario, Feature feature, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, int level = 1)
        {
            this.step = step;
            this.scenario = scenario;
            this.feature = feature;
            this.NativeSuggestionItem = nativeSuggestionItemFactory.Create(step.Text, GetInsertionText(step), level, BindingType.ToString().Substring(0, 1), this);
        }

        private const string stepParamIndent = "         ";

        static internal string GetInsertionText(ScenarioStep step)
        {
            if (step.TableArg == null && step.MultiLineTextArgument == null)
                return step.Text;

            StringBuilder result = new StringBuilder(step.Text);
            if (step.MultiLineTextArgument != null)
            {
                result.AppendLine();
                result.Append(stepParamIndent);
                result.AppendLine("\"\"\"");
                result.AppendLine(stepParamIndent);
                result.Append(stepParamIndent);
                result.Append("\"\"\"");
            }
            if (step.TableArg != null)
            {
                result.AppendLine();
                result.Append(stepParamIndent);
                result.Append("|");
                foreach (var cell in step.TableArg.Header.Cells)
                {
                    result.Append(" ");
                    result.Append(cell.Value);
                    result.Append(" |");
                }
                result.AppendLine();
                result.Append(stepParamIndent);
                result.Append("|");
                foreach (var cell in step.TableArg.Header.Cells)
                {
                    result.Append(" ");
                    result.Append(' ', cell.Value.Length);
                    result.Append(" |");
                }
            }
            return result.ToString();
        }

        public bool Match(StepBinding binding, bool includeRegexCheck)
        {
            if (includeRegexCheck && binding.Regex != null && !binding.Regex.Match(StepText).Success)
                return false;

            //TODO: consider scope
            return BindingType == binding.BindingType;
        }

    }
}