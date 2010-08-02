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
        private readonly TableBuilder tableBuilder = new TableBuilder();

        public ExampleBuilder(string name, string description, FilePosition position)
        {
            this.text = TextHelper.GetText(name, description);
            this.position = position;
        }

        public ExampleSet GetResult()
        {
            Table exampleTable = tableBuilder.GetResult();
            if (exampleTable == null)
                throw new SpecFlowParserException(
                    new ErrorDetail
                    {
                        Line = position.Line,
                        Column = position.Column,
                        Message = "No examples defined in the example set!"
                    });

            return new ExampleSet(text, exampleTable);
        }

        public void ProcessTableRow(string[] row, FilePosition rowPosition)
        {
            tableBuilder.ProcessTableRow(row, rowPosition);
        }
    }
}