using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.UnitTestConverter;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.SpecFlowPlugin
{
    public interface IExternalDataFeaturePatcher
    {
        SpecFlowDocument PatchDocument(SpecFlowDocument feature);
    }

    public class ExternalDataFeaturePatcher : IExternalDataFeaturePatcher
    {
        public const string PROPERTY_TAG = "property";
        private readonly ITagFilterMatcher _tagFilterMatcher;
        private readonly ITestDataProvider _testDataProvider;

        public ExternalDataFeaturePatcher(SpecFlowProjectConfiguration configuration, ITagFilterMatcher tagFilterMatcher, ITestDataProvider testDataProvider)
        {
            _tagFilterMatcher = tagFilterMatcher;
            _testDataProvider = testDataProvider;
        }


        public SpecFlowDocument PatchDocument(SpecFlowDocument originalSpecFlowDocument)
        {
            var feature = originalSpecFlowDocument.SpecFlowFeature;
            var scenarioDefinitions = feature.Children.Where(c => c is Background).ToList();

            foreach (var scenario in feature.ScenarioDefinitions.OfType<Scenario>())
            {

                if (!_tagFilterMatcher.GetTagValue(PROPERTY_TAG, scenario.Tags.Select(t => t.Name.Substring(1)),
                    out var tagString))
                {
                    scenarioDefinitions.Add(scenario);
                    continue;
                }

                var newScenarioOutline = PatchScenario(tagString, scenario);

                scenarioDefinitions.Add(newScenarioOutline);
            }

            var newDocument = CreateSpecFlowDocument(originalSpecFlowDocument, feature, scenarioDefinitions);
            return newDocument;
        }

        private ScenarioOutline PatchScenario(string tagString, Scenario scenario)
        {
            var tokens = tagString.Split('=');
            var propertyName = tokens[0];
            var propertyValueExpression = tokens[1];

            var propertyValueTokens = propertyValueExpression.Split('.');

            var testValues = _testDataProvider.TestData;
            foreach (var propertyValueToken in propertyValueTokens)
            {
                var dict = testValues as Dictionary<string, object>;
                if (dict == null )
                    throw new InvalidOperationException($"Cannot resolve properties from {testValues}");

                if (!dict.TryGetValue(propertyValueToken, out testValues) &&
                    !dict.TryGetValue(propertyValueToken.Replace("_", " "), out testValues))
                    throw new InvalidOperationException($"Cannot resolve property {propertyValueToken}");
            }

            var examples = new List<Examples>(scenario.Examples);

            var header = new TableRow(scenario.Location, new TableCell[]
            {
                new TableCell(scenario.Location, "Variant"),
                new TableCell(scenario.Location, propertyName)
            });

            var testValuesDict = testValues as Dictionary<string, object>;

            var rows = testValuesDict
                .Select(p => new TableRow(scenario.Location, new[]
                {
                    new TableCell(scenario.Location, p.Key),
                    new TableCell(scenario.Location, p.Value.ToString()),
                })).ToArray();


            var newExample = new Examples(new Tag[0], scenario.Location, "Examples", string.Empty, string.Empty, header, rows);
            examples.Add(newExample);


            var newScenarioOutline = new ScenarioOutline(scenario.Tags.ToArray(),
                scenario.Location,
                scenario.Keyword,
                scenario.Name,
                scenario.Description,
                scenario.Steps.ToArray(),
                examples.ToArray());
            return newScenarioOutline;
        }


        private SpecFlowDocument CreateSpecFlowDocument(SpecFlowDocument originalSpecFlowDocument, SpecFlowFeature originalFeature, List<IHasLocation> scenarioDefinitions)
        {
            var newFeature = new SpecFlowFeature(originalFeature.Tags.ToArray(),
                                                 originalFeature.Location,
                                                 originalFeature.Language,
                                                 originalFeature.Keyword,
                                                 originalFeature.Name,
                                                 originalFeature.Description,
                                                 scenarioDefinitions.ToArray());

            var newDocument = new SpecFlowDocument(newFeature, originalSpecFlowDocument.Comments.ToArray(), originalSpecFlowDocument.DocumentLocation);
            return newDocument;
        }
    }
}