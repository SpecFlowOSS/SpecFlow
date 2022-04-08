using System;
using System.Collections.Generic;
using System.Linq;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Transformation
{
    public abstract class ScenarioTransformation : GherkinDocumentVisitor
    {
        protected SpecFlowDocument _sourceDocument;
        private SpecFlowDocument _transformedDocument;
        private SpecFlowFeature _transformedFeature;
        private bool _hasTransformedScenarioInFeature = false;
        private bool _hasTransformedScenarioInCurrentRule = false;
        private readonly List<IHasLocation> _featureChildren = new();
        private readonly List<IHasLocation> _ruleChildren = new();
        private List<IHasLocation> _currentChildren;

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
            _ruleChildren.Clear();
            _hasTransformedScenarioInFeature = false;
            _hasTransformedScenarioInCurrentRule = false;
            _currentChildren = _featureChildren;
        }

        protected abstract Scenario GetTransformedScenarioOutline(ScenarioOutline scenarioOutline);
        protected abstract Scenario GetTransformedScenario(Scenario scenario);

        protected override void OnScenarioOutlineVisited(ScenarioOutline scenarioOutline)
        {
            var transformedScenarioOutline = GetTransformedScenarioOutline(scenarioOutline);
            OnScenarioVisitedInternal(scenarioOutline, transformedScenarioOutline);
        }

        protected override void OnScenarioVisited(Scenario scenario)
        {
            var transformedScenario = GetTransformedScenario(scenario);
            OnScenarioVisitedInternal(scenario, transformedScenario);
        }

        private void OnScenarioVisitedInternal(Scenario scenario, Scenario transformedScenario)
        {
            if (transformedScenario == null)
            {
                _currentChildren.Add(scenario);
                return;
            }

            _hasTransformedScenarioInFeature = true;
            _hasTransformedScenarioInCurrentRule = true;
            _currentChildren.Add(transformedScenario);
        }

        protected override void OnBackgroundVisited(Background background)
        {
            _featureChildren.Add(background);
        }

        protected override void OnRuleVisiting(Rule rule)
        {
            _ruleChildren.Clear();
            _hasTransformedScenarioInCurrentRule = false;
            _currentChildren = _ruleChildren;
        }

        protected override void OnRuleVisited(Rule rule)
        {
            _currentChildren = _featureChildren;
            if (_hasTransformedScenarioInCurrentRule)
            {
                var transformedRule = new Rule(
                    rule.Tags?.ToArray() ?? Array.Empty<Tag>(),
                    rule.Location,
                    rule.Keyword,
                    rule.Name,
                    rule.Description,
                    _ruleChildren.ToArray());
                _featureChildren.Add(transformedRule);
            }
            else
            {
                _featureChildren.Add(rule);
            }
        }

        protected override void OnFeatureVisited(Feature feature)
        {
            if (_hasTransformedScenarioInFeature)
                _transformedFeature = new SpecFlowFeature(
                    feature.Tags?.ToArray() ?? Array.Empty<Tag>(),
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
