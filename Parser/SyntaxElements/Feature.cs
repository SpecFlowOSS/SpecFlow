using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    /// <summary>
    /// not required in the final AST
    /// </summary>
    public class DescriptionLine
    {
        public string LineText { get; set; }

        public DescriptionLine(Text lineText)
        {
            LineText = lineText.Value;
        }
    }

    public class Feature
    {
        public Tags Tags { get; set; }
        public Background Background { get; set; }
        public Scenario[] Scenarios { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public Feature()
        {
        }

        public Feature(Text title, Tags tags, DescriptionLine[] description, Background background, params Scenario[] scenarios)
        {
            Tags = tags;
            Description = description == null ? string.Empty : string.Join(Environment.NewLine, description.Select(d => d.LineText.Trim()).ToArray());
            Background = background;
            Scenarios = scenarios;
            Title = title.Value;
        }
    }
}