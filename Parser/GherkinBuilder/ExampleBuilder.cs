using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ExampleBuilder : ITableProcessor
    {
        private readonly string text;
        private readonly FilePosition position;

        private Dictionary<int, string[]> _exampleLines = new Dictionary<int, string[]>();

        public ExampleBuilder(string text, FilePosition position)
        {
            this.text = text;
            this.position = position;
        }

        public ExampleSet GetResult()
        {
            Table exampleTable = new Table
            {
                Header = new Row
                                {
                                    Cells = _exampleLines.Values.First().Select(c => new Cell(new Text(c))).ToArray(),
                                    FilePosition = new FilePosition(_exampleLines.Keys.First(), 1)
                                },
                Body = _exampleLines.Skip(1).Select(r => new Row
                                                                {
                                                                    Cells = r.Value.Select(c => new Cell(new Text(c))).ToArray(),
                                                                    FilePosition = new FilePosition(r.Key, 1)
                                                                }).ToArray()
            };
            return new ExampleSet(new Text(text), exampleTable);
        }

        public void ProcessTableRow(string[] row, int lineNumber)
        {
            _exampleLines.Add(lineNumber, row);
        }
    }
}