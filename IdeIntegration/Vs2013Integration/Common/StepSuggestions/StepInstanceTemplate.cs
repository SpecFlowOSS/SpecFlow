using System;
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Infrastructure;
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
        public StepDefinitionType StepDefinitionType { get { return Template.StepDefinitionType; } }

        public BoundInstanceTemplate(StepInstanceTemplate<TNativeSuggestionItem> template, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory, IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> suggestions)
        {
            Template = template;
            this.suggestions = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory, suggestions);
            NativeSuggestionItem = nativeSuggestionItemFactory.CloneTo(template.NativeSuggestionItem, this);
        }

        public CultureInfo Language
        {
            get { return Template.Language; } 
        }

        public bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService)
        {
            if (binding.StepDefinitionType != StepDefinitionType)
                return false;

            if (suggestions.Count == 0)
                return false;

            return suggestions.Any(i => i.Match(binding, bindingCulture, true, stepDefinitionMatchService));
        }
    }

    public class StepInstanceTemplate<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>
    {
        private readonly StepSuggestionList<TNativeSuggestionItem> instances;
        public IEnumerable<IBoundStepSuggestion<TNativeSuggestionItem>> Instances { get { return instances; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; private set; }

        public StepDefinitionType StepDefinitionType { get; private set; }
        internal string StepPrefix { get; private set; }

        public CultureInfo Language { get; private set; }

        public bool Match(IStepDefinitionBinding binding, CultureInfo bindingCulture, bool includeRegexCheck, IStepDefinitionMatchService stepDefinitionMatchService)
        {
            if (binding.StepDefinitionType != StepDefinitionType)
                return false;

            if (instances.Count == 0)
                return false;

            return instances.Any(i => i.Match(binding, bindingCulture, true, stepDefinitionMatchService));
        }

        static private readonly Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");

        public StepInstanceTemplate(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, Feature feature, StepContext stepContext, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            StepDefinitionType = (StepDefinitionType)scenarioStep.ScenarioBlock;
            Language = stepContext.Language;

            NativeSuggestionItem = nativeSuggestionItemFactory.Create(scenarioStep.Text, StepInstance<TNativeSuggestionItem>.GetInsertionText(scenarioStep), 1, StepDefinitionType.ToString().Substring(0, 1) + "-t", this);
            instances = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
            AddInstances(scenarioStep, scenarioOutline, feature, stepContext, nativeSuggestionItemFactory);

            var match = paramRe.Match(scenarioStep.Text);
            StepPrefix = match.Success ? scenarioStep.Text.Substring(0, match.Index) : scenarioStep.Text;
        }

        private void AddInstances(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, Feature feature, StepContext stepContext, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
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
                    instances.Add(new StepInstance<TNativeSuggestionItem>(newStep, feature, stepContext, nativeSuggestionItemFactory, 2) { ParentTemplate = this });
                }
            }
        }

        static public bool IsTemplate(ScenarioStep scenarioStep)
        {
            return paramRe.Match(scenarioStep.Text).Success;
        }
    }
}