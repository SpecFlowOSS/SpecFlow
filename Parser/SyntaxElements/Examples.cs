using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Examples
    {
        public ExampleSet[] ExampleSets { get; set; }

        public Examples()
        {
        }

        public Examples(params ExampleSet[] exampleSets)
        {
            ExampleSets = exampleSets;
        }
    }

    public class ExampleSet
    {
        public Tags Tags { get; set; }
        public string Keyword { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public GherkinTable Table { get; set; }

        public ExampleSet()
        {
        }

        public ExampleSet(string keyword, string title, string description, Tags tags, GherkinTable table)
        {
            Keyword = keyword;
            Title = title ?? string.Empty;
            Description = description;
            Tags = tags;
            Table = table;
        }
    }
}