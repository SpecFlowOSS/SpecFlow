using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class ScenarioOutline : Scenario
    {
        public Examples Examples { get; set; }

        public ScenarioOutline()
        {
        }

        public ScenarioOutline(string title, Tags tags, ScenarioSteps scenarioSteps, Examples examples) : base(title, tags, scenarioSteps)
        {
            Examples = examples;
        }
    }
}