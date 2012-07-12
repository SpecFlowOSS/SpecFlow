using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public interface ISourceFilePosition
    {
        string SourceFile { get; }
        FilePosition FilePosition { get; }
    }

    public class StepInstance<TNativeSuggestionItem> : StepInstance, IBoundStepSuggestion<TNativeSuggestionItem>, ISourceFilePosition
    {
        private readonly List<BoundStepSuggestions<TNativeSuggestionItem>> matchGroups = new List<BoundStepSuggestions<TNativeSuggestionItem>>(1);
        public ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get { return matchGroups; } }

        public CultureInfo Language
        {
            get { return StepContext.Language; }
        }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }
        public StepInstanceTemplate<TNativeSuggestionItem> ParentTemplate { get; internal set; }

        public string SourceFile { get; private set; }
        public FilePosition FilePosition { get; private set; }

        public StepInstance(ScenarioStep step, Feature feature, StepContext stepContext, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, int level = 1)
            : base((StepDefinitionType)step.ScenarioBlock, (StepDefinitionKeyword)step.StepKeyword, step.Keyword, step.Text, stepContext)
        {
            this.NativeSuggestionItem = nativeSuggestionItemFactory.Create(step.Text, GetInsertionText(step), level, StepDefinitionType.ToString().Substring(0, 1), this);
            this.FilePosition = step.FilePosition;
            this.SourceFile = feature.SourceFile;
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

        public bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService)
        {
            return stepDefinitionMatchService.Match(binding, this, bindingCulture, useRegexMatching: includeRegexCheck, useParamMatching: false).Success;
        }
    }
}