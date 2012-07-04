using System;
using System.Linq;
using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public interface IStepSuggestionGroup<TNativeSuggestionItem>
    {
        IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Suggestions { get; }
    }

    public class BoundStepSuggestions<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>, IStepSuggestionGroup<TNativeSuggestionItem>
    {
        private readonly StepSuggestionList<TNativeSuggestionItem> suggestions;
        public IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Suggestions { get { return suggestions; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }

        public IStepDefinitionBinding StepBinding { get; private set; }
        public StepDefinitionType StepDefinitionType { get; set; }

        public BoundStepSuggestions(StepDefinitionType stepDefinitionType, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            StepBinding = null;
            StepDefinitionType = stepDefinitionType;
            NativeSuggestionItem = nativeSuggestionItemFactory.Create("[unbound steps]", "...", 0, "nb", this);
            suggestions = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
        }

        public BoundStepSuggestions(IStepDefinitionBinding stepBinding, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            if (stepBinding == null) throw new ArgumentNullException("stepBinding");

            StepBinding = stepBinding;
            StepDefinitionType = stepBinding.StepDefinitionType;
            string suggestionText = GetSuggestionText(stepBinding);
            NativeSuggestionItem = nativeSuggestionItemFactory.Create(suggestionText, GetInsertionText(StepBinding), 0, StepDefinitionType.ToString().Substring(0, 1) + "-b", this);
            suggestions = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
        }

        private string GetSuggestionText(IStepDefinitionBinding stepBinding)
        {
            string suggestionTextBase = stepBinding.Regex == null ? "[...]" :
                "[" + RegexSampler.GetRegexSample(stepBinding.Regex.ToString(), stepBinding.Method.Parameters.Select(p => p.ParameterName).ToArray()) + "]";

            return string.Format("{0} -> {1}", suggestionTextBase, stepBinding.Method.GetShortDisplayText());
        }

        private string GetInsertionText(IStepDefinitionBinding stepBinding)
        {
            if (stepBinding.Regex == null)
                return "...";

            var paramNames = stepBinding.Method.Parameters.Select(p => p.ParameterName);
            return RegexSampler.GetRegexSample(stepBinding.Regex.ToString(), paramNames.ToArray());
        }

        public void AddSuggestion(IBoundStepSuggestion<TNativeSuggestionItem> stepSuggestion)
        {
            suggestions.Add(stepSuggestion);
            stepSuggestion.MatchGroups.Add(this);
        }

        public void RemoveSuggestion(IBoundStepSuggestion<TNativeSuggestionItem> stepSuggestion)
        {
            suggestions.Remove(stepSuggestion);
            stepSuggestion.MatchGroups.Remove(this);
        }
    }
}