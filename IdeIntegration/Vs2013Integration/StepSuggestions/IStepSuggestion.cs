using System.Collections.Generic;
using System.Globalization;
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
        CultureInfo Language { get; }

        bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService);
        ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get; }
    }
}