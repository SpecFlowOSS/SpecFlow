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

        public ScenarioOutline(string title, string description, Tags tags, ScenarioSteps scenarioSteps, Examples examples) : 
            base(title, description, tags, scenarioSteps)
        {
            Examples = examples;
        }
    }
}