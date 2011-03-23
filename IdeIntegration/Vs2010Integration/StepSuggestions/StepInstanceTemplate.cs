using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public class BoundInstanceTemplate<TNativeSuggestionItem> : IBoundStepSuggestion<TNativeSuggestionItem>, IStepSuggestionGroup<TNativeSuggestionItem>
    {
        public StepInstanceTemplate<TNativeSuggestionItem> Template { get; private set; }

        private readonly List<BoundStepSuggestions<TNativeSuggestionItem>> matchGroups = new List<BoundStepSuggestions<TNativeSuggestionItem>>(1);
        public ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get { return matchGroups; } }

        private readonly StepSuggestionList<TNativeSuggestionItem> suggestions;
        public IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Suggestions { get { return suggestions; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }
        public BindingType BindingType { get { return Template.BindingType; } }

        public BoundInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> template, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> suggestions)
        {
            Template = template;
            this.suggestions = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory, suggestions);
            NativeSuggestionItem = nativeSuggestionItemFactory.CloneTo(template.NativeSuggestionItem, this);
        }

        public bool Match(StepBinding binding, bool includeRegexCheck, IBindingMatchService bindingMatchService)
        {
            if (binding.BindingType != BindingType)
                return false;

            if (suggestions.Count == 0)
                return false;

            return suggestions.Any(i => i.Match(binding, true, bindingMatchService));
        }
    }

    public class StepInstanceTemplate<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>
    {
        private readonly StepSuggestionList<TNativeSuggestionItem> instances;
        public IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Instances { get { return instances; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }

        public BindingType BindingType { get; private set; }
        internal string StepPrefix { get; private set; }

        public bool Match(StepBinding binding, bool includeRegexCheck, IBindingMatchService bindingMatchService)
        {
            if (binding.BindingType != BindingType)
                return false;

            if (instances.Count == 0)
                return false;

            return instances.Any(i => i.Match(binding, true, bindingMatchService));
        }

        static private readonly Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");

        public StepInstanceTemplate(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, StepScope stepScope, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            BindingType = (BindingType)scenarioStep.ScenarioBlock;

            NativeSuggestionItem = nativeSuggestionItemFactory.Create(scenarioStep.Text, StepInstance<TNativeSuggestionItem>.GetInsertionText(scenarioStep), 1, BindingType.ToString().Substring(0, 1) + "-t", this);
            instances = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
            AddInstances(scenarioStep, scenarioOutline, stepScope, nativeSuggestionItemFactory);

            var match = paramRe.Match(scenarioStep.Text);
            StepPrefix = match.Success ? scenarioStep.Text.Substring(0, match.Index) : scenarioStep.Text;
        }

        private void AddInstances(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, StepScope stepScope, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            foreach (var exampleSet in scenarioOutline.Examples.ExampleSets)
            {
                foreach (var row in exampleSet.Table.Body)
                {
                    var replacedText = paramRe.Replace(scenarioStep.Text,
                        match =>
                        {
                            string param = match.Groups["param"].Value;
                            int headerIndex = Array.FindIndex(exampleSet.Table.Header.Cells, c => c.Value.Equals(param));
                            if (headerIndex < 0)
                                return match.Value;
                            return row.Cells[headerIndex].Value;
                        });

                    var newStep = scenarioStep.Clone();
                    newStep.Text = replacedText;
                    instances.Add(new StepInstance<TNativeSuggestionItem>(newStep, stepScope, nativeSuggestionItemFactory, 2) { ParentTemplate = this });
                }
            }
        }

        static public bool IsTemplate(ScenarioStep scenarioStep)
        {
            return paramRe.Match(scenarioStep.Text).Success;
        }
    }
}