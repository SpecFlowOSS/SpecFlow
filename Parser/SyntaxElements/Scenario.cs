using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    [XmlInclude(typeof(ScenarioOutline))]
    public class Scenario
    {
        public Tags Tags { get; set; }
        public string Title { get; set; }
        public FilePosition FilePosition { get; set; }
        public ScenarioSteps Steps { get; set; }

        public Scenario()
        {
        }

        public Scenario(Text title, Tags tags, ScenarioSteps scenarioSteps)
        {
            Title = title.Value;
            Tags = tags;
            Steps = scenarioSteps ?? new ScenarioSteps();
        }
    }
}
