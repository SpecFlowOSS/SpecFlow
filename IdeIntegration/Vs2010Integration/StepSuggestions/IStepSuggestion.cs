using System.Collections.Generic;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public interface IStepSuggestion<out TNativeSuggestionItem>
    {
        TNativeSuggestionItem NativeSuggestionItem { get; }
        StepDefinitionType StepDefinitionType { get; }
    }

    public interface IBoundStepSuggestion<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>
    {
        bool Match(StepDefinitionBinding binding, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService);
        ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get; }
    }
}