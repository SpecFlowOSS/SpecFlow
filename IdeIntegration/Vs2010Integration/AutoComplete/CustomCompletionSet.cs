using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;

namespace TechTalk.SpecFlow.Vs2010Integration.AutoComplete
{
    internal class CustomCompletionSet : CompletionSet
    {
        public bool PrefixMatch { get; set; }

        public CustomCompletionSet()
        {
            PrefixMatch = true;
        }

        public CustomCompletionSet(string moniker, string displayName, ITrackingSpan applicableTo, IEnumerable<Completion> completions, IEnumerable<Completion> completionBuilders)
            : base(moniker, displayName, applicableTo, completions, completionBuilders)
        {
            PrefixMatch = true;
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

            if (PrefixMatch)
            {
                return displayText.StartsWith(this._filterBufferText, !this._filterCaseSensitive, CultureInfo.CurrentCulture);
            }

            StringComparison comparison = _filterCaseSensitive
                                              ? StringComparison.CurrentCulture
                                              : StringComparison.CurrentCultureIgnoreCase;

            if (string.IsNullOrWhiteSpace(_filterBufferText))
                return false;

            var words = _filterBufferText.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
            if (words.Length < 1)
                return false; // empty

            return words.All(word => WordPrefixContains(displayText, word, comparison)); // all words should match
        }

        private bool WordPrefixContains(string text, string wordPrefix, StringComparison comparison)
        {
            var wordIndex = text.IndexOf(wordPrefix, comparison);
            while (wordIndex > 0 && char.IsLetterOrDigit(text[wordIndex - 1]))
            {
                wordIndex = text.IndexOf(wordPrefix, wordIndex + 1, comparison);    
            }

            return wordIndex >= 0;
        }
    }
}