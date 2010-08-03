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
        public string Title { get; set; }
        public string Description { get; set; }
        public Table Table { get; set; }

        public ExampleSet()
        {
        }

        public ExampleSet(string title, string description, Table table)
        {
            Title = title ?? string.Empty;
            Description = description;
            Table = table;
        }
    }
}