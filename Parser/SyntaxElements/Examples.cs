using System;
using System.Linq;

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
        public Table Table { get; set; }

        public ExampleSet()
        {
        }

        public ExampleSet(string title, Table table)
        {
            Title = title ?? string.Empty;
            Table = table;
        }
    }
}