using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Gherkin.Ast;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSources;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Transformation
{
    public class IncludeExternalDataTransformation : ScenarioTransformation
    {
        private readonly ISpecificationProvider _specificationProvider;
        private CultureInfo _featureCultureInfo = null;

        public IncludeExternalDataTransformation(ISpecificationProvider specificationProvider)
        {
            _specificationProvider = specificationProvider;
        }

        protected override void OnFeatureVisiting(Feature feature)
        {
            base.OnFeatureVisiting(feature);
            _featureCultureInfo = GetFeatureCultureInfo(feature.Language);
        }

        private CultureInfo GetFeatureCultureInfo(string featureLanguage)
        {
            try
            {
                if (featureLanguage == null || featureLanguage.Equals("en", StringComparison.InvariantCultureIgnoreCase)) 
                    return new CultureInfo("en-US");
                var result = new CultureInfo(featureLanguage);
                if (result.IsNeutralCulture) 
                    CultureInfo.CreateSpecificCulture(result.Name);
                return result;
            }
            catch (Exception)
            {
                return new CultureInfo("en-US");
            }
        }

        private IEnumerable<Tag> GetSourceFeatureTags() => 
            _sourceDocument.SpecFlowFeature.Tags ?? new Tag[0];

        protected override Scenario GetTransformedScenario(Scenario scenario)
        {
            var tags = GetSourceFeatureTags().Concat(scenario.Tags);
            var specification = _specificationProvider.GetSpecification(tags, _sourceDocument.SourceFilePath);
            if (specification == null)
            {
                Debug.WriteLine($"No DataSource specification for '{scenario.Keyword}: {scenario.Name}' (Scenario)");
                return null;
            }

            return GetTransformedScenario(scenario, specification);
        }

        protected override Scenario GetTransformedScenarioOutline(ScenarioOutline scenarioOutline)
        {
            if (scenarioOutline.Examples == null || !scenarioOutline.Examples.Any())
                return GetTransformedScenario(scenarioOutline);

            var tags = GetSourceFeatureTags()
                .Concat(scenarioOutline.Tags ?? new Tag[0])
                .Concat(scenarioOutline.Examples.SelectMany(e => e.Tags ?? new Tag[0]));
            
            var specification = _specificationProvider.GetSpecification(tags, _sourceDocument.SourceFilePath);
            if (specification == null)
            {
                Debug.WriteLine($"No DataSource specification for '{scenarioOutline.Keyword}: {scenarioOutline.Name}' (Scenario Outline)");
                return null;
            }

            var firstExamples = scenarioOutline.Examples.First();
            var tableHeader = firstExamples.TableHeader;
            var examplesHeaderNames = tableHeader.Cells.Select(c => c.Value).ToArray();

            return GetTransformedScenario(scenarioOutline, specification, examplesHeaderNames, firstExamples.Keyword);
        }

        private Scenario GetTransformedScenario(Scenario scenario, ExternalDataSpecification specification, string[] examplesHeaderNames = null, string examplesKeyword = "Examples")
        {
            var exampleRecords = specification.GetExampleRecords(examplesHeaderNames);
            var exampleRows = exampleRecords.Items
                                            .Select(rec => new TableRow(null, exampleRecords.Header.Select(h => new TableCell(null, rec.Fields[h].AsString(_featureCultureInfo))).ToArray()))
                                            .ToArray();

            var examplesBlock = CreateExamplesBlock(exampleRecords.Header, exampleRows, examplesKeyword);
            Examples[] examples = scenario.Examples == null ? 
                new[] { examplesBlock } : 
                scenario.Examples.Concat(new[] { examplesBlock }).ToArray();

            return new ScenarioOutline(
                scenario.Tags.ToArray(),
                scenario.Location,
                scenario.Keyword,
                scenario.Name,
                scenario.Description,
                scenario.Steps.ToArray(),
                examples
            );
        }

        private Examples CreateExamplesBlock(string[] headerNames, TableRow[] exampleRows, string keyword)
        {
            keyword ??= "External Examples";
            var name = "External Examples";
            var tableHeader = new TableRow(null, headerNames.Select(h => new TableCell(null, h)).ToArray());
            return new Examples(new Tag[0], null, keyword, name, "", tableHeader, exampleRows);
        }
    }
}
