using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public abstract class StepSuggestionProvider<TNativeSuggestionItem>
    {
        protected readonly INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory;
        protected readonly RegexDictionary<BoundStepSuggestions<TNativeSuggestionItem>> boundStepSuggestions;
        private readonly Dictionary<BindingType, BoundStepSuggestions<TNativeSuggestionItem>> notMatchingSteps;

        protected abstract IBindingMatchService BindingMatchService { get; }

        public IEnumerable<TNativeSuggestionItem> GetNativeSuggestionItems(BindingType bindingType)
        {
            var suggestions = boundStepSuggestions.Where(s => s.BindingType == bindingType).Append(notMatchingSteps[bindingType]);
            return GetNativeSuggestionItems(suggestions);
        }

        private IEnumerable<TNativeSuggestionItem> GetNativeSuggestionItems(IEnumerable<IStepSuggestion<TNativeSuggestionItem>> suggestions, string lastInsertionText = null)
        {
            foreach (var suggestion in suggestions)
            {
                var insertionText = nativeSuggestionItemFactory.GetInsertionText(suggestion.NativeSuggestionItem);
                if (insertionText == null || insertionText.Equals(lastInsertionText))
                    continue;

                yield return suggestion.NativeSuggestionItem;
                lastInsertionText = insertionText;

                IStepSuggestionGroup<TNativeSuggestionItem> suggestionGroup = suggestion as IStepSuggestionGroup<TNativeSuggestionItem>;
                if (suggestionGroup != null)
                {
                    foreach (var nativeSuggestionItem in GetNativeSuggestionItems(suggestionGroup.Suggestions, lastInsertionText))
                        yield return nativeSuggestionItem;
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

            var affectedSuggestions = new List<IBoundStepSuggestion<TNativeSuggestionItem>>(
                boundStepSuggestions.GetRelatedItems(stepBinding.Regex).SelectMany(relatedItem => relatedItem.Suggestions).Where(s => s.Match(stepBinding, true, BindingMatchService)));
            affectedSuggestions.AddRange(notMatchingSteps[item.BindingType].Suggestions.Where(s => s.Match(stepBinding, true, BindingMatchService)));

            foreach (var affectedSuggestion in affectedSuggestions)
                RemoveBoundStepSuggestion(affectedSuggestion);

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

            boundStepSuggestions.Remove(item);
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
            if (suggestion is StepInstanceTemplate<TNativeSuggestionItem>)
                RemoveStepInstanceTemplate((StepInstanceTemplate<TNativeSuggestionItem>)suggestion);
            if (suggestion is IBoundStepSuggestion<TNativeSuggestionItem>)
                RemoveBoundStepSuggestion((IBoundStepSuggestion<TNativeSuggestionItem>)suggestion);
        }

        private void RemoveBoundStepSuggestion(IBoundStepSuggestion<TNativeSuggestionItem> suggestion)
        {
            foreach (var item in suggestion.MatchGroups.ToArray())
            {
                item.RemoveSuggestion(suggestion);
            }
        }

        public void AddStepInstance(StepInstance<TNativeSuggestionItem> stepInstance)
        {
            var matchingItems = GetMatchingBoundStepSuggestions(stepInstance);
            AddStepSuggestionToMatchingItems(stepInstance.BindingType, matchingItems, item => item.AddSuggestion(stepInstance));
        }

        private IEnumerable<BoundStepSuggestions<TNativeSuggestionItem>> GetMatchingBoundStepSuggestions(StepInstance stepInstance)
        {
            return boundStepSuggestions.GetMatchingItems(stepInstance.Text).Where(it => BindingMatchService.Match(it.StepBinding, stepInstance, useRegexMatching: false, useParamMatching: false).Success);
        }

        public void AddStepInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            var matchingItems = GetMatchingBindings(stepInstanceTemplate); 
            AddStepSuggestionToMatchingItems(stepInstanceTemplate.BindingType, matchingItems, 
                item => item.AddSuggestion(
                    new BoundInstanceTemplate<TNativeSuggestionItem>(
                        stepInstanceTemplate, 
                        nativeSuggestionItemFactory,
                        stepInstanceTemplate.Instances.Where(s => item.StepBinding == null || s.Match(item.StepBinding, true, BindingMatchService)))));
        }

        private void RemoveStepInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            var matchingItems = GetMatchingBindings(stepInstanceTemplate).Append(notMatchingSteps[stepInstanceTemplate.BindingType]); 

            var boundInstanceTemplates = matchingItems.SelectMany(item => item.Suggestions)
                .OfType<BoundInstanceTemplate<TNativeSuggestionItem>>()
                .Where(bt => bt.Template == stepInstanceTemplate);

            foreach (var boundInstanceTemplate in boundInstanceTemplates.ToArray())
                RemoveBoundStepSuggestion(boundInstanceTemplate);
        }

        private IEnumerable<BoundStepSuggestions<TNativeSuggestionItem>> GetMatchingBindings(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            //TODO: optimize
            return boundStepSuggestions.GetPotentialItemsByPrefix(stepInstanceTemplate.StepPrefix).Where(it => stepInstanceTemplate.Match(it.StepBinding, true, BindingMatchService)); 
        }

        private void AddStepSuggestionToMatchingItems(BindingType bindingType, IEnumerable<BoundStepSuggestions<TNativeSuggestionItem>> matchingItems, 
            Action<BoundStepSuggestions<TNativeSuggestionItem>> addAction)
        {
            bool wasMatching = false;
            foreach (var item in matchingItems)
            {
                addAction(item);
                wasMatching = true;
            }

            if (!wasMatching)
            {
                addAction(notMatchingSteps[bindingType]);
            }
        }

        public IEnumerable<StepBinding> GetConsideredBindings(string stepText)
        {
            return boundStepSuggestions.GetMatchingItems(stepText).Select(it => it.StepBinding);
        }
    }
}
