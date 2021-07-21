using System;
using Gherkin.Ast;
using TechTalk.SpecFlow.Parser;

namespace SpecFlow.ExternalData.SpecFlowPlugin.Transformation
{
    public abstract class GherkinDocumentVisitor
    {
        protected virtual void AcceptDocument(SpecFlowDocument document)
        {
            OnDocumentVisiting(document);
            if (document.Feature != null)
            {
                AcceptFeature(document.Feature);
            }
            OnDocumentVisited(document);
        }

        protected virtual void AcceptFeature(Feature feature)
        {
            OnFeatureVisiting(feature);
            foreach (var featureChild in feature.Children)
            {
                if (featureChild is Rule rule) AcceptRule(rule);
                else if (featureChild is Background background) AcceptBackground(background);
                else if (featureChild is ScenarioOutline scenarioOutline) AcceptScenarioOutline(scenarioOutline);
                else if (featureChild is Scenario scenario) AcceptScenario(scenario);
            }
            OnFeatureVisited(feature);
        }

        protected virtual void AcceptStep(Step step)
        {
            OnStepVisited(step);
        }

        protected virtual void AcceptScenario(Scenario scenario)
        {
            OnScenarioVisiting(scenario);
            foreach (var step in scenario.Steps)
            {
                AcceptStep(step);
            }
            OnScenarioVisited(scenario);
        }

        protected virtual void AcceptScenarioOutline(ScenarioOutline scenarioOutline)
        {
            OnScenarioOutlineVisiting(scenarioOutline);
            foreach (var step in scenarioOutline.Steps)
            {
                AcceptStep(step);
            }
            OnScenarioOutlineVisited(scenarioOutline);
        }

        protected virtual void AcceptBackground(Background background)
        {
            OnBackgroundVisiting(background);
            foreach (var step in background.Steps)
            {
                AcceptStep(step);
            }
            OnBackgroundVisited(background);
        }

        protected virtual void AcceptRule(Rule rule)
        {
            OnRuleVisiting(rule);
            foreach (var ruleChild in rule.Children)
            {
                if (ruleChild is ScenarioOutline scenarioOutline) AcceptScenarioOutline(scenarioOutline);
                else if (ruleChild is Scenario scenario) AcceptScenario(scenario);
            }
            OnRuleVisited(rule);
        }

        protected virtual void OnDocumentVisiting(SpecFlowDocument document)
        {
            //nop
        }

        protected virtual void OnDocumentVisited(SpecFlowDocument document)
        {
            //nop
        }

        protected virtual void OnFeatureVisiting(Feature feature)
        {
            //nop
        }

        protected virtual void OnFeatureVisited(Feature feature)
        {
            //nop
        }

        protected virtual void OnBackgroundVisiting(Background background)
        {
            //nop
        }

        protected virtual void OnBackgroundVisited(Background background)
        {
            //nop
        }

        protected virtual void OnRuleVisiting(Rule rule)
        {
            //nop
        }

        protected virtual void OnRuleVisited(Rule rule)
        {
            //nop
        }

        protected virtual void OnScenarioOutlineVisiting(ScenarioOutline scenarioOutline)
        {
            //nop
        }

        protected virtual void OnScenarioOutlineVisited(ScenarioOutline scenarioOutline)
        {
            //nop
        }

        protected virtual void OnScenarioVisiting(Scenario scenario)
        {
            //nop
        }

        protected virtual void OnScenarioVisited(Scenario scenario)
        {
            //nop
        }

        protected virtual void OnStepVisited(Step step)
        {
            //nop
        }
    }
}
