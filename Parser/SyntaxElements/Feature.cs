using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Feature
    {
        public string Language { get; set; }
        public string SourceFile { get; set; }

        public Tags Tags { get; set; }
        public Background Background { get; set; }
        public Scenario[] Scenarios { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Feature()
        {
        }

        public Feature(string title, Tags tags, string description, Background background, params Scenario[] scenarios)
        {
            Tags = tags;
            Description = description ?? string.Empty;
            Background = background;
            Scenarios = scenarios;
            Title = title;
        }
    }
}