using System;
using System.Collections;
using System.Collections.Generic;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
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

            private StepInstanceTemplate<TNativeSuggestionItem> GetParentTemplate(IStepSuggestion<TNativeSuggestionItem> stepSuggestion)
            {
                StepInstance<TNativeSuggestionItem> stepInstance = stepSuggestion as StepInstance<TNativeSuggestionItem>;
                return stepInstance != null ? stepInstance.ParentTemplate : null;
            }

            public int Compare(IStepSuggestion<TNativeSuggestionItem> x, IStepSuggestion<TNativeSuggestionItem> y)
            {
                var parent1 = GetParentTemplate(x);
                var parent2 = GetParentTemplate(y);
                if (parent1 != parent2)
                {
                    if (parent1 == y)
                        return 1;
                    if (parent2 == x)
                        return -1;

                    if (parent1 != null && parent2 != null)
                        return Compare(parent1, parent2);

                    if (parent1 != null)
                        return Compare(parent1, y);
                    if (parent2 != null)
                        return Compare(x, parent2);
                }


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