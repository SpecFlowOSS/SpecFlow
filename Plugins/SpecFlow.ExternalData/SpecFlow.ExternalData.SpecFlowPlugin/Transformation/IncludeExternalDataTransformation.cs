using System;
using System.Diagnostics;
using System.Linq;
using Gherkin.Ast;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Transformation
{
    public class IncludeExternalDataTransformation : ScenarioTransformation
    {
        private readonly ISpecificationProvider _specificationProvider;

        public IncludeExternalDataTransformation(ISpecificationProvider specificationProvider)
        {
            _specificationProvider = specificationProvider;
        }

        protected override Scenario GetTransformedScenarioOutline(ScenarioOutline scenarioOutline)
        {
            var specification = _specificationProvider.GetSpecification(scenarioOutline.Tags, _sourceDocument.SourceFilePath); //TODO: add parent tags
            if (specification == null)
            {
                Debug.WriteLine($"No DataSource specification for '{scenarioOutline.Keyword}: {scenarioOutline.Name}'");
                return null;
            }

            var firstExamples = scenarioOutline.Examples.FirstOrDefault();
            Debug.Assert(firstExamples != null); //TODO: handle
            var tableHeader = firstExamples.TableHeader;
            //TODO: handle not provided columns
            var examplesHeaderNames = tableHeader.Cells.Select(c => c.Value).ToArray();

            var exampleRecords = specification.GetExampleRecords(examplesHeaderNames);
            var exampleRows = exampleRecords.Items
                .Select(rec => new TableRow(null, exampleRecords.Header.Select(h => new TableCell(null, rec.Fields[h].AsString)).ToArray()))
                .ToArray();
            
            return new ScenarioOutline(
                scenarioOutline.Tags.ToArray(),
                scenarioOutline.Location,
                scenarioOutline.Keyword,
                scenarioOutline.Name,
                scenarioOutline.Description,
                scenarioOutline.Steps.ToArray(),
                scenarioOutline.Examples.Concat(new[] { new Examples(new Tag[0], null, firstExamples.Keyword, "External Data", "", tableHeader, exampleRows) }).ToArray()
            );
        }

        protected override Scenario GetTransformedScenario(Scenario scenario) => null;
    }
}
