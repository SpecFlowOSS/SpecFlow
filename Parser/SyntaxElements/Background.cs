using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Background
    {
        public string Title { get; set; }
        public ScenarioSteps Steps { get; set; }

        public FilePosition FilePosition { get; set; }

        public Background()
        {
        }

        public Background(string title, ScenarioSteps scenarioSteps)
        {
            Title = title ?? "";
            Steps = scenarioSteps ?? new ScenarioSteps();
        }
    }
}