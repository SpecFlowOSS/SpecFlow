using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Background
    {
        public string Keyword { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public ScenarioSteps Steps { get; set; }

        public FilePosition FilePosition { get; set; }

        public Background()
        {
        }

        public Background(string keyword, string title, string description, ScenarioSteps scenarioSteps)
        {
            Keyword = keyword;
            Title = title ?? "";
            Description = description;
            Steps = scenarioSteps ?? new ScenarioSteps();
        }
    }
}