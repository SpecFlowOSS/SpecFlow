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
        public string StatusText { get; set; }

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
            var filterText = ApplicableTo.GetText(ApplicableTo.TextBuffer.CurrentSnapshot).TrimEnd();
            var directMatchItems = Completions.Where(c => DoesCompletionMatchApplicabilityTextDirect(c, filterText, CompletionMatchType.MatchInsertionText, false)).ToArray();
            if (directMatchItems.Any())
            {
                this.SelectionStatus = new CompletionSelectionStatus(directMatchItems[0], true, directMatchItems.Length == 1);
            }
            else
            {
                this.SelectionStatus = new CompletionSelectionStatus(null, false, false);
            }
        }

        protected void CustomFilter(CompletionMatchType matchType, bool caseSensitive)
        {
            ITextSnapshot currentSnapshot = ApplicableTo.TextBuffer.CurrentSnapshot;
            var filterText = ApplicableTo.GetText(currentSnapshot).TrimEnd();
            if (string.IsNullOrEmpty(filterText))
            {
                ((FilteredObservableCollection<Completion>)Completions).StopFiltering();
                ((FilteredObservableCollection<Completion>)CompletionBuilders).StopFiltering();
            }
            else
            {
                ((FilteredObservableCollection<Completion>)Completions).Filter(completion => DoesCompletionMatchApplicabilityText(completion, filterText, matchType, caseSensitive));
                ((FilteredObservableCollection<Completion>)CompletionBuilders).Filter(completion => DoesCompletionMatchApplicabilityText(completion, filterText, matchType, caseSensitive));
            }
        }

        protected virtual bool DoesCompletionMatchApplicabilityText(Completion completion, string filterText, CompletionMatchType matchType, bool caseSensitive)
        {
            return DoesCompletionMatchApplicabilityTextDirect(completion, filterText, matchType, caseSensitive);
        }

        protected virtual bool DoesCompletionMatchApplicabilityTextDirect(Completion completion, string filterText, CompletionMatchType matchType, bool caseSensitive)
        {
            string displayText = string.Empty;
            if (matchType == CompletionMatchType.MatchDisplayText)
            {
                displayText = completion.DisplayText;
            }
            else if (matchType == CompletionMatchType.MatchInsertionText)
            {
                displayText = completion.InsertionText;
            }

            if (PrefixMatch)
            {
                return displayText.StartsWith(filterText, !caseSensitive, CultureInfo.CurrentCulture);
            }

            StringComparison comparison = caseSensitive
                                              ? StringComparison.CurrentCulture
                                              : StringComparison.CurrentCultureIgnoreCase;

            if (string.IsNullOrWhiteSpace(filterText))
                return false;

            var words = filterText.Split(new[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
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