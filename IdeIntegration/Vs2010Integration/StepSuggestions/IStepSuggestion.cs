using System.Collections.Generic;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public interface IStepSuggestion<out TNativeSuggestionItem>
    {
        TNativeSuggestionItem NativeSuggestionItem { get; }
        BindingType BindingType { get; }
    }

    public interface IBoundStepSuggestion<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>
    {
        bool Match(StepBinding binding, bool includeRegexCheck);
        ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get; }
    }
}