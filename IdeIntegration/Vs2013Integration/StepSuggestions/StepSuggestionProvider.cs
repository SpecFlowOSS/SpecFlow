using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Bindings.Reflection;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public abstract class StepSuggestionProvider<TNativeSuggestionItem>
    {
        protected readonly IProjectScope projectScope;
        protected readonly INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory;
        protected readonly RegexDictionary<BoundStepSuggestions<TNativeSuggestionItem>> boundStepSuggestions;
        private readonly Dictionary<StepDefinitionType, BoundStepSuggestions<TNativeSuggestionItem>> notMatchingSteps;

        protected abstract IStepDefinitionMatchService BindingMatchService { get; }

        public IEnumerable<TNativeSuggestionItem> GetNativeSuggestionItems(StepDefinitionType stepDefinitionType)
        {
            var suggestions = boundStepSuggestions.Where(s => s.StepDefinitionType == stepDefinitionType).Append(notMatchingSteps[stepDefinitionType]);
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

        protected StepSuggestionProvider(INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, IProjectScope projectScope)
        {
            boundStepSuggestions = new RegexDictionary<BoundStepSuggestions<TNativeSuggestionItem>>(item => item.StepBinding == null ? null : item.StepBinding.Regex);
            notMatchingSteps = new Dictionary<StepDefinitionType, BoundStepSuggestions<TNativeSuggestionItem>>
                                    {
                                        {StepDefinitionType.Given, new BoundStepSuggestions<TNativeSuggestionItem>(StepDefinitionType.Given, nativeSuggestionItemFactory)},
                                        {StepDefinitionType.When, new BoundStepSuggestions<TNativeSuggestionItem>(StepDefinitionType.When, nativeSuggestionItemFactory)},
                                        {StepDefinitionType.Then, new BoundStepSuggestions<TNativeSuggestionItem>(StepDefinitionType.Then, nativeSuggestionItemFactory)}
                                    };
            this.nativeSuggestionItemFactory = nativeSuggestionItemFactory;
            this.projectScope = projectScope;
        }

        public void AddBinding(IStepDefinitionBinding stepBinding)
        {
            try
            {
                var item = new BoundStepSuggestions<TNativeSuggestionItem>(stepBinding, nativeSuggestionItemFactory);

                var affectedSuggestions = new List<IBoundStepSuggestion<TNativeSuggestionItem>>(
                    boundStepSuggestions.GetRelatedItems(stepBinding.Regex).SelectMany(relatedItem => relatedItem.Suggestions).Where(s => s.Match(stepBinding, GetBindingCulture(s), true, BindingMatchService)));
                affectedSuggestions.AddRange(notMatchingSteps[item.StepDefinitionType].Suggestions.Where(s => s.Match(stepBinding, GetBindingCulture(s), true, BindingMatchService)));

                foreach (var affectedSuggestion in affectedSuggestions)
                    RemoveBoundStepSuggestion(affectedSuggestion);

                boundStepSuggestions.Add(item);

                foreach (var affectedSuggestion in affectedSuggestions)
                    AddStepSuggestion(affectedSuggestion);
            }
            catch(Exception ex)
            {
                projectScope.Tracer.Trace("Error while adding step definition binding: " + ex, GetType().Name);
                throw;
            }
        }

        public void RemoveBinding(IStepDefinitionBinding stepBinding)
        {
            var item = boundStepSuggestions.GetRelatedItems(stepBinding.Regex).FirstOrDefault(it => it.StepBinding == stepBinding);
            if (item == null)
                return;

            foreach (var stepSuggestion in item.Suggestions.ToArray())
            {
                item.RemoveSuggestion(stepSuggestion);
                if (!stepSuggestion.MatchGroups.Any())
                {
                    notMatchingSteps[item.StepDefinitionType].AddSuggestion(stepSuggestion);
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
            AddStepSuggestionToMatchingItems(stepInstance.StepDefinitionType, matchingItems, item => item.AddSuggestion(stepInstance));
        }

        private IEnumerable<BoundStepSuggestions<TNativeSuggestionItem>> GetMatchingBoundStepSuggestions(StepInstance stepInstance)
        {
            return boundStepSuggestions.GetMatchingItems(stepInstance.Text).Where(it => BindingMatchService.Match(it.StepBinding, stepInstance, GetBindingCulture(stepInstance), useRegexMatching: false, useParamMatching: false).Success);
        }

        private CultureInfo GetBindingCulture(CultureInfo featureLanguage)
        {
            return projectScope.SpecFlowProjectConfiguration.RuntimeConfiguration.BindingCulture ?? featureLanguage;
        }

        private CultureInfo GetBindingCulture(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            return GetBindingCulture(stepInstanceTemplate.Language);
        }

        private CultureInfo GetBindingCulture(IBoundStepSuggestion<TNativeSuggestionItem> boundStepSuggestion)
        {
            return GetBindingCulture(boundStepSuggestion.Language);
        }

        private CultureInfo GetBindingCulture(StepInstance stepInstance)
        {
            return GetBindingCulture(stepInstance.StepContext.Language);
        }

        public void AddStepInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            var matchingItems = GetMatchingBindings(stepInstanceTemplate); 
            AddStepSuggestionToMatchingItems(stepInstanceTemplate.StepDefinitionType, matchingItems, 
                item => item.AddSuggestion(
                    new BoundInstanceTemplate<TNativeSuggestionItem>(
                        stepInstanceTemplate, 
                        nativeSuggestionItemFactory,
                        stepInstanceTemplate.Instances.Where(s => item.StepBinding == null || s.Match(item.StepBinding, GetBindingCulture(s), true, BindingMatchService)))));
        }

        private void RemoveStepInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            var matchingItems = GetMatchingBindings(stepInstanceTemplate).Append(notMatchingSteps[stepInstanceTemplate.StepDefinitionType]); 

            var boundInstanceTemplates = matchingItems.SelectMany(item => item.Suggestions)
                .OfType<BoundInstanceTemplate<TNativeSuggestionItem>>()
                .Where(bt => bt.Template == stepInstanceTemplate);

            foreach (var boundInstanceTemplate in boundInstanceTemplates.ToArray())
                RemoveBoundStepSuggestion(boundInstanceTemplate);
        }

        private IEnumerable<BoundStepSuggestions<TNativeSuggestionItem>> GetMatchingBindings(StepInstanceTemplate<TNativeSuggestionItem> stepInstanceTemplate)
        {
            //TODO: optimize
            return boundStepSuggestions.GetPotentialItemsByPrefix(stepInstanceTemplate.StepPrefix).Where(it => stepInstanceTemplate.Match(it.StepBinding, GetBindingCulture(stepInstanceTemplate), true, BindingMatchService)); 
        }

        private void AddStepSuggestionToMatchingItems(StepDefinitionType stepDefinitionType, IEnumerable<BoundStepSuggestions<TNativeSuggestionItem>> matchingItems, 
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
                addAction(notMatchingSteps[stepDefinitionType]);
            }
        }

        public IEnumerable<IStepDefinitionBinding> GetConsideredStepDefinitions(StepDefinitionType stepDefinitionType, string stepText = null)
        {
            return boundStepSuggestions.GetMatchingItems(stepText).Select(it => it.StepBinding).Where(sd => sd.StepDefinitionType == stepDefinitionType);
        }

        public IEnumerable<StepInstance> GetMatchingInstances(IBindingMethod method)
        {
            var instances = boundStepSuggestions
                .Where(bss => bss.StepBinding.Method.MethodEquals(method))
                .SelectMany(bss => bss.Suggestions.Select(s => 
                    s is StepInstance ? (StepInstance)s : 
                    s is BoundInstanceTemplate<TNativeSuggestionItem> ? ((BoundInstanceTemplate<TNativeSuggestionItem>)s).Suggestions.OfType<StepInstance>().FirstOrDefault() 
                    : null))
                .Where(si => si != null);

            return instances;
        }
    }
}
