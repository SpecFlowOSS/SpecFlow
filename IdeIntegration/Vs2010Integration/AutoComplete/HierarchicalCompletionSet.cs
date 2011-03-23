using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using TechTalk.SpecFlow.Vs2010Integration.StepSuggestions;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete
{
    internal class CustomCompletionSet : CompletionSet
    {
        public CustomCompletionSet()
        {
        }

        public CustomCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, IEnumerable<Completion> completions, IEnumerable<Completion> completionBuilders)
            : base(moniker, displayName, applicableTo, completions, completionBuilders)
        {
        }

        public override void Filter()
        {
            CustomFilter(CompletionMatchType.MatchInsertionText, false);
        }

        public override void SelectBestMatch()
        {
            SelectBestMatch(CompletionMatchType.MatchInsertionText, false);
        }

        protected string _filterBufferText;
        protected bool _filterCaseSensitive;
        protected CompletionMatchType _filterMatchType;

        protected void CustomFilter(CompletionMatchType matchType, bool caseSensitive)
        {
            ITextSnapshot currentSnapshot = ApplicableTo.TextBuffer.CurrentSnapshot;
            this._filterBufferText = ApplicableTo.GetText(currentSnapshot).TrimEnd();
            if (string.IsNullOrEmpty(this._filterBufferText))
            {
                ((FilteredObservableCollection<Completion>)Completions).StopFiltering();
                ((FilteredObservableCollection<Completion>)CompletionBuilders).StopFiltering();
            }
            else
            {
                this._filterMatchType = matchType;
                this._filterCaseSensitive = caseSensitive;
                ((FilteredObservableCollection<Completion>)Completions).Filter(DoesCompletionMatchApplicabilityText);
                ((FilteredObservableCollection<Completion>)CompletionBuilders).Filter(DoesCompletionMatchApplicabilityText);
            }
        }

        protected virtual bool DoesCompletionMatchApplicabilityText(Completion completion)
        {
            string displayText = string.Empty;
            if (this._filterMatchType == CompletionMatchType.MatchDisplayText)
            {
                displayText = completion.DisplayText;
            }
            else if (this._filterMatchType == CompletionMatchType.MatchInsertionText)
            {
                displayText = completion.InsertionText;
            }
            return displayText.StartsWith(this._filterBufferText, !this._filterCaseSensitive, CultureInfo.CurrentCulture);
        }
    }

    internal class HierarchicalCompletionSet : CustomCompletionSet
    {
        public HierarchicalCompletionSet()
        {
        }

        public HierarchicalCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, IEnumerable<Completion> completions, IEnumerable<Completion> completionBuilders) : base(moniker, displayName, applicableTo, completions, completionBuilders)
        {
        }

        protected override bool DoesCompletionMatchApplicabilityText(Completion completion)
        {
            if (base.DoesCompletionMatchApplicabilityText(completion))
                return true;

            object parentObject = null;
            completion.Properties.TryGetProperty<object>("parentObject", out parentObject);
            IStepSuggestionGroup<Completion> parentObjectAsGroup = parentObject as IStepSuggestionGroup<Completion>;
            return 
                parentObjectAsGroup != null && 
                parentObjectAsGroup.Suggestions
                    .Any(stepSuggestion => stepSuggestion.NativeSuggestionItem != null && DoesCompletionMatchApplicabilityText(stepSuggestion.NativeSuggestionItem));
        }
    }
}