using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Background
    {
        public string Title { get; set; }
        public ScenarioSteps Steps { get; set; }

        public Background()
        {
        }

        public Background(Text title, ScenarioSteps scenarioSteps)
        {
            Title = title == null ? "" : title.Value;
            Steps = scenarioSteps ?? new ScenarioSteps();
        }
    }
}