using Gherkin.Ast;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Generator.Generation;

public class ScenarioDefinitionInFeatureFile
{
    public StepsContainer ScenarioDefinition { get; }
    public Rule Rule { get; }
    public SpecFlowFeature Feature { get; }

    public ScenarioOutline ScenarioOutline => ScenarioDefinition as ScenarioOutline;
    public Scenario Scenario => ScenarioDefinition as Scenario;

    public bool IsScenarioOutline => ScenarioOutline != null;

    public ScenarioDefinitionInFeatureFile(StepsContainer stepsContainer, SpecFlowFeature feature, Rule rule = null)
    {
        ScenarioDefinition = stepsContainer;
        Feature = feature;
        Rule = rule;
    }
}
