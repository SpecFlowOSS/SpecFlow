using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Gherkin.Ast;
using SpecFlow.ExternalData.SpecFlowPlugin.DataSource;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Transformation
{
    public class IncludeExternalDataTransformation : GherkinDocumentVisitor
    {
        private readonly ISpecificationProvider _specificationProvider;

        private SpecFlowDocument _sourceDocument;
        private SpecFlowDocument _transformedDocument;
        private SpecFlowFeature _transformedFeature;
        private bool _hasTransformedScenario = false;
        private readonly List<IHasLocation> _featureChildren = new();

        public IncludeExternalDataTransformation(ISpecificationProvider specificationProvider)
        {
            _specificationProvider = specificationProvider;
        }

        public SpecFlowDocument TransformDocument(SpecFlowDocument document)
        {
            Reset();
            AcceptDocument(document);
            return _transformedDocument ?? document;
        }

        private void Reset()
        {
            _sourceDocument = null;
            _transformedDocument = null;
            _transformedFeature = null;
            _featureChildren.Clear();
            _hasTransformedScenario = false;
        }

        protected override void OnScenarioOutlineVisited(ScenarioOutline scenarioOutline)
        {
            //TODO: filter tags
            var specification = _specificationProvider.GetSpecification(scenarioOutline.Tags, _sourceDocument.SourceFilePath); //TODO: add parent tags
            if (specification == null)
            {
                Debug.WriteLine($"No DataSource specification for '{scenarioOutline.Keyword}: {scenarioOutline.Name}'");
                _featureChildren.Add(scenarioOutline);
                return;
            }
            Debug.Assert(specification != null); //TODO: filter?
            
            var firstExamples = scenarioOutline.Examples.FirstOrDefault();
            Debug.Assert(firstExamples != null); //TODO: handle
            var tableHeader = firstExamples.TableHeader;
            //TODO: handle not provided columns
            var exampleRows = specification.GetExampleRecords()
                .Select(rec => new TableRow(null, tableHeader.Cells.Select(hc => new TableCell(null, rec.Fields[hc.Value].AsString)).ToArray()))
                .ToArray();
            var transformedScenarioOutline = new ScenarioOutline(
                scenarioOutline.Tags.ToArray(),
                scenarioOutline.Location,
                scenarioOutline.Keyword,
                scenarioOutline.Name,
                scenarioOutline.Description,
                scenarioOutline.Steps.ToArray(),
                scenarioOutline.Examples.Concat(new[] { new Examples(new Tag[0], null, firstExamples.Keyword, "External Data", "", tableHeader, exampleRows) }).ToArray()
            );

            _hasTransformedScenario = true;
            _featureChildren.Add(transformedScenarioOutline);
        }

        protected override void OnScenarioVisited(Scenario scenario)
        {
            _featureChildren.Add(scenario);
        }

        protected override void OnBackgroundVisited(Background background)
        {
            _featureChildren.Add(background);
        }

        protected override void OnRuleVisited(Rule rule)
        {
            throw new NotImplementedException();
        }

        protected override void OnFeatureVisited(Feature feature)
        {
            if (_hasTransformedScenario)
                _transformedFeature = new SpecFlowFeature(
                    feature.Tags.ToArray(),
                    feature.Location,
                    feature.Language,
                    feature.Keyword,
                    feature.Name,
                    feature.Description,
                    _featureChildren.ToArray());
        }

        protected override void OnDocumentVisiting(SpecFlowDocument document)
        {
            _sourceDocument = document;
        }

        protected override void OnDocumentVisited(SpecFlowDocument document)
        {
            if (_transformedFeature != null)
                _transformedDocument = new SpecFlowDocument(_transformedFeature, document.Comments.ToArray(), document.DocumentLocation);
        }
    }
}
