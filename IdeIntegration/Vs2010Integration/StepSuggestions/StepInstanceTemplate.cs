using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow.Parser.SyntaxElements;
using TechTalk.SpecFlow.Vs2010Integration.Bindings;

namespace TechTalk.SpecFlow.Vs2010Integration.StepSuggestions
{
    public class StepInstanceTemplate<TNativeSuggestionItem> : IStepSuggestion<TNativeSuggestionItem>, IStepSuggestionGroup<TNativeSuggestionItem>
    {
        private readonly List<BoundStepSuggestions<TNativeSuggestionItem>> matchGroups = new List<BoundStepSuggestions<TNativeSuggestionItem>>(1);
        public ICollection<BoundStepSuggestions<TNativeSuggestionItem>> MatchGroups { get { return matchGroups; } }

        private readonly StepSuggestionList<TNativeSuggestionItem> instances;
        public IEnumerable<IStepSuggestion<TNativeSuggestionItem>> Suggestions { get { return instances; } }

        public TNativeSuggestionItem NativeSuggestionItem { get; set; }

        public BindingType BindingType { get; private set; }

        public bool Match(StepBinding binding, bool includeRegexCheck)
        {
            if (binding.BindingType != BindingType)
                return false;

            if (instances.Count == 0)
                return false;

            return instances.Any(i => i.Match(binding, true));
        }

        static private readonly Regex paramRe = new Regex(@"\<(?<param>[^\>]+)\>");

        public StepInstanceTemplate(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, Feature feature, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
        {
            BindingType = (BindingType)scenarioStep.ScenarioBlock;

            NativeSuggestionItem = nativeSuggestionItemFactory.Create(scenarioStep.Text, scenarioStep.Text, 1, this, BindingType.ToString().Substring(0, 1) + "-t");
            instances = new StepSuggestionList<TNativeSuggestionItem>(nativeSuggestionItemFactory);
            AddInstances(scenarioStep, scenarioOutline, feature, nativeSuggestionItemFactory);
        }

        private void AddInstances(ScenarioStep scenarioStep, ScenarioOutline scenarioOutline, Feature feature, INativeSuggestionItemFactory<TNativeSuggestionItem> nativeSuggestionItemFactory)
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
                    instances.Add(new StepInstance<TNativeSuggestionItem>(newStep, scenarioOutline, feature, nativeSuggestionItemFactory, 2) { ParentTemplate = this });
                }
            }
        }

        static public bool IsTemplate(ScenarioStep scenarioStep)
        {
            return paramRe.Match(scenarioStep.Text).Success;
        }
    }
}