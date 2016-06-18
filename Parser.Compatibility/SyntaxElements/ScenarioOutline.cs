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

        public ScenarioOutline(string keyword, string title, string description, Tags tags, ScenarioSteps scenarioSteps, Examples examples) : 
            base(keyword, title, description, tags, scenarioSteps)
        {
            Examples = examples;
        }
    }
}