using System;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using TechTalk.SpecFlow.Utils;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public interface IStepSuggestionGroup<TNativeSuggestionItem>
    {
        IEnumerable<IStepSuggestion<TNativeSuggestionItem>> Suggestions { get; }
    }

    public class BoundStepSuggestions<TNativeSuggestionItem> : IStepSuggestionGroup<TNativeSuggestionItem>
    {
        private readonly StepSuggestionList<TNativeSuggestionItem> suggestions;
        public IEnumerable<IStepSuggestion<TNativeSuggestionItem>> Suggestions { get { return suggestions; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }

        public StepBinding StepBinding { get; private set; }
        public BindingType BindingType { get; set; }

        public BoundStepSuggestions(BindingType bindingType, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            StepBinding = null;
            BindingType = bindingType;
            NativeSuggestionItem = nativeSuggestionItemFactory.Create("[unbound steps]", "...", 0, this);
            suggestions = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
        }

        public BoundStepSuggestions(StepBinding stepBinding, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            if (stepBinding == null) throw new ArgumentNullException("stepBinding");

            StepBinding = stepBinding;
            BindingType = stepBinding.BindingType;
            string suggestionText = GetSuggestionText(stepBinding);
            NativeSuggestionItem = nativeSuggestionItemFactory.Create(suggestionText, GetInsertionText(StepBinding), 0, this);
            suggestions = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
        }

        private string GetSuggestionText(StepBinding stepBinding)
        {
            string suggestionTextBase = stepBinding.Regex == null ? "[...]" :
                "[" + RegexSampler.GetRegexSample(stepBinding.Regex.ToString(), stepBinding.Method.Parameters.Select(p => p.ParameterName).ToArray()) + "]";

            return string.Format("{0} -> {1}", suggestionTextBase, stepBinding.Method.ShortDisplayText);
        }

        private string GetInsertionText(StepBinding stepBinding)
        {
            if (stepBinding.Regex == null)
                return "...";

            var paramNames = stepBinding.Method.Parameters.Select(p => p.ParameterName);
            return RegexSampler.GetRegexSample(stepBinding.Regex.ToString(), paramNames.ToArray());
        }

        public void AddSuggestion(IStepSuggestion<TNativeSuggestionItem> stepSuggestion)
        {
            suggestions.Add(stepSuggestion);
            stepSuggestion.MatchGroups.Add(this);
        }

        public void RemoveSuggestion(IStepSuggestion<TNativeSuggestionItem> stepSuggestion)
        {
            suggestions.Remove(stepSuggestion);
            stepSuggestion.MatchGroups.Remove(this);
        }
    }

    internal class StepSuggestionList<TNativeSuggestionItem> : ICollection<IStepSuggestion<TNativeSuggestionItem>>
    {
        private readonly List<IStepSuggestion<TNativeSuggestionItem>> items = new List<IStepSuggestion<TNativeSuggestionItem>>();
        private readonly InsertionTextComparer insertionTextComparer;

        private class InsertionTextComparer : IComparer<IStepSuggestion<TNativeSuggestionItem>>
        {
            private readonly INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory;

            public InsertionTextComparer(INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
            {
                this.nativeSuggestionItemFactory = nativeSuggestionItemFactory;
            }

            public int Compare(IStepSuggestion<TNativeSuggestionItem> x, IStepSuggestion<TNativeSuggestionItem> y)
            {
                string p1 = nativeSuggestionItemFactory.GetInsertionText(x.NativeSuggestionItem);
                string p2 = nativeSuggestionItemFactory.GetInsertionText(y.NativeSuggestionItem);
                return string.Compare(p1, p2, StringComparison.CurrentCultureIgnoreCase);
            }
        }

        public StepSuggestionList(INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            insertionTextComparer = new InsertionTextComparer(nativeSuggestionItemFactory);
        }

        public void Add(IStepSuggestion<TNativeSuggestionItem> item)
        {
            int index = items.BinarySearch(item, insertionTextComparer);
            if (index < 0)
                index = ~index;

            items.Insert(index, item);
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool Contains(IStepSuggestion<TNativeSuggestionItem> item)
        {
            return items.Contains(item);
        }

        public void CopyTo(IStepSuggestion<TNativeSuggestionItem>[] array, int arrayIndex)
        {
            items.CopyTo(array, arrayIndex);
        }

        public bool Remove(IStepSuggestion<TNativeSuggestionItem> item)
        {
            return items.Remove(item);
        }

        public int Count
        {
            get { return items.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public IEnumerator<IStepSuggestion<TNativeSuggestionItem>> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}