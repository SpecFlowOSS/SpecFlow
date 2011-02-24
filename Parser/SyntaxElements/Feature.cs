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
        public string Keyword { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public FilePosition FilePosition { get; set; }
        public Background Background { get; set; }
        public Scenario[] Scenarios { get; set; }
        public Comment[] Comments { get; set; }

        public Feature()
        {
        }

        public Feature(string keyword, string title, Tags tags, string description, Background background, Scenario[] scenarios, Comment[] comments)
        {
            Keyword = keyword;
            Title = title;
            Tags = tags;
            Description = description ?? string.Empty;
            Background = background;
            Scenarios = scenarios;
            Comments = comments;
        }
    }
}