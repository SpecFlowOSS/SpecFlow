using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public class StepSuggestionProvider<TNativeSuggestionItem>
    {
        protected readonly INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory;
        private readonly RegexDictionary<BoundStepSuggestions<TNativeSuggestionItem>> boundStepSuggestions;
        private readonly Dictionary<BindingType, BoundStepSuggestions<TNativeSuggestionItem>> notMatchingSteps;

        public IEnumerable<TNativeSuggestionItem> GetNativeSuggestionItems(BindingType bindingType)
        {
            string lastInsertionText = null;
            foreach (var boundStepSuggestion in boundStepSuggestions.Where(s => s.BindingType == bindingType).Append(notMatchingSteps[bindingType]))
            {
                yield return boundStepSuggestion.NativeSuggestionItem;
                lastInsertionText = nativeSuggestionItemFactory.GetInsertionText(boundStepSuggestion.NativeSuggestionItem);
                foreach (var stepSuggestion in boundStepSuggestion.Suggestions)
                {
                    var insertionText = nativeSuggestionItemFactory.GetInsertionText(stepSuggestion.NativeSuggestionItem);
                    if (!lastInsertionText.Equals(insertionText))
                    {
                        yield return stepSuggestion.NativeSuggestionItem;
                        lastInsertionText = insertionText;
                    }
                }
            }
        }

        public StepSuggestionProvider(INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            boundStepSuggestions = new RegexDictionary<BoundStepSuggestions<TNativeSuggestionItem>>(item => item.StepBinding == null ? null : item.StepBinding.Regex);
            notMatchingSteps = new Dictionary<BindingType, BoundStepSuggestions<TNativeSuggestionItem>>()
                                    {
                                        {BindingType.Given, new BoundStepSuggestions<TNativeSuggestionItem>(BindingType.Given, nativeSuggestionItemFactory)},
                                        {BindingType.When, new BoundStepSuggestions<TNativeSuggestionItem>(BindingType.When, nativeSuggestionItemFactory)},
                                        {BindingType.Then, new BoundStepSuggestions<TNativeSuggestionItem>(BindingType.Then, nativeSuggestionItemFactory)}
                                    };
            this.nativeSuggestionItemFactory = nativeSuggestionItemFactory;
        }

        public void AddBinding(StepBinding stepBinding)
        {
            var item = new BoundStepSuggestions<TNativeSuggestionItem>(stepBinding, nativeSuggestionItemFactory);

            List<IStepSuggestion<TNativeSuggestionItem>> affectedSuggestions = new List<IStepSuggestion<TNativeSuggestionItem>>(
                boundStepSuggestions.GetRelatedItems(stepBinding.Regex).SelectMany(relatedItem => relatedItem.Suggestions).Where(s => s.Match(stepBinding, true)));
            affectedSuggestions.AddRange(notMatchingSteps[item.BindingType].Suggestions.Where(s => s.Match(stepBinding, true)));

            foreach (var affectedSuggestion in affectedSuggestions)
                RemoveStepSuggestion(affectedSuggestion);

            boundStepSuggestions.Add(item);

            foreach (var affectedSuggestion in affectedSuggestions)
                AddStepSuggestion(affectedSuggestion);
        }

        public void RemoveBinding(StepBinding stepBinding)
        {
            var item = boundStepSuggestions.GetRelatedItems(stepBinding.Regex).FirstOrDefault(it => it.StepBinding == stepBinding);
            if (item == null)
                return;

            foreach (var stepSuggestion in item.Suggestions.ToArray())
            {
                item.RemoveSuggestion(stepSuggestion);
                if (!stepSuggestion.MatchGroups.Any())
                {
                    notMatchingSteps[item.BindingType].AddSuggestion(stepSuggestion);
                }
            }
        }

        public void AddStepSuggestion(IStepSuggestion<TNativeSuggestionItem> suggestion)
        {
            if (suggestion is StepInstanceTemplate<TNativeSuggestionItem>)
                AddStepInstanceTemplate((StepInstanceTemplate<TNativeSuggestionItem>)suggestion);
            if (suggestion is StepInstance<TNativeSuggestionItem>)
                AddStepInstance((StepInstance<TNativeSuggestionItem>)suggestion);
        }

        public void RemoveStepSuggestion(IStepSuggestion<TNativeSuggestionItem> suggestion)
        {
            foreach (var item in suggestion.MatchGroups.ToArray())
            {
                item.RemoveSuggestion(suggestion);
            }
        }

        public void AddStepInstance(StepInstance<TNativeSuggestionItem> stepInstance)
        {
            var matchingItems = boundStepSuggestions.GetMatchingItems(stepInstance.StepText).Where(it => stepInstance.Match(it.StepBinding, false));
            AddStepSuggestionToMatchingItems(stepInstance, matchingItems);
        }

        public void AddStepInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            var matchingItems = boundStepSuggestions.Where(it => stepInstanceTemplate.Match(it.StepBinding, true)); //TODO: optimize
            AddStepSuggestionToMatchingItems(stepInstanceTemplate, matchingItems);

            //TODO: temporary solution
            foreach (StepInstance<TNativeSuggestionItem> instance in stepInstanceTemplate.Suggestions)
            {
                AddStepInstance(instance);
            }
        }

        private void AddStepSuggestionToMatchingItems(IStepSuggestion<TNativeSuggestionItem> suggestion, IEnumerable<BoundStepSuggestions<TNativeSuggestionItem>> matchingItems)
        {
            bool wasMatching = false;
            foreach (var item in matchingItems)
            {
                item.AddSuggestion(suggestion);
                wasMatching = true;
            }

            if (!wasMatching)
            {
                notMatchingSteps[suggestion.BindingType].AddSuggestion(suggestion);
            }
        }
    }
}
