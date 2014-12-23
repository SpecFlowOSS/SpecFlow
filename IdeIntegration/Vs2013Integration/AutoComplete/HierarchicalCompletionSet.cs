﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete
{
    internal class HierarchicalCompletionSet : CustomCompletionSet
    {
        public HierarchicalCompletionSet()
        {
            PrefixMatch = false;
        }

        public HierarchicalCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, IEnumerable<Completion> completions, IEnumerable<Completion> completionBuilders) : base(moniker, displayName, applicableTo, completions, completionBuilders)
        {
            PrefixMatch = false;
        }

        protected override bool DoesCompletionMatchApplicabilityText(Completion completion, string filterText, CompletionMatchType matchType, bool caseSensitive)
        {
            if (base.DoesCompletionMatchApplicabilityText(completion, filterText, matchType, caseSensitive))
                return true;

            object parentObject;
            completion.Properties.TryGetProperty("parentObject", out parentObject);
            IStepSuggestionGroup<Completion> parentObjectAsGroup = parentObject as IStepSuggestionGroup<Completion>;
            return 
                parentObjectAsGroup != null && 
                parentObjectAsGroup.Suggestions
                    .Any(stepSuggestion => stepSuggestion.NativeSuggestionItem != null && DoesCompletionMatchApplicabilityText(stepSuggestion.NativeSuggestionItem, filterText, matchType, caseSensitive));
        }
    }
}