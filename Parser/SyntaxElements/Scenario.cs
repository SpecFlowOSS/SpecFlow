using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    [XmlInclude(typeof(ScenarioOutline))]
    public class Scenario
    {
        public Tags Tags { get; set; }
        public string Keyword { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public FilePosition FilePosition { get; set; }
        public ScenarioSteps Steps { get; set; }

        public Scenario()
        {
        }

        public Scenario(string keyword, string title, string description, Tags tags, ScenarioSteps scenarioSteps)
        {
            Keyword = keyword;
            Title = title;
            Description = description;
            Tags = tags;
            Steps = scenarioSteps ?? new ScenarioSteps();
        }
    }
}
