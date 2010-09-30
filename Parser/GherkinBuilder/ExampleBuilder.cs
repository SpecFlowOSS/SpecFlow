using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ExampleBuilder : ITableProcessor
    {
        private readonly string name;
        private readonly string description;
        private readonly FilePosition position;
        private readonly TableBuilder tableBuilder = new TableBuilder();

        public string Title
        {
            get { return name; }
        }

        public FilePosition Position
        {
            get { return position; }
        }

        public ExampleBuilder(string name, string description, FilePosition position)
        {
            this.name = name;
            this.description = description;
            this.position = position;
        }

        public ExampleSet GetResult()
        {
            Table exampleTable = tableBuilder.GetResult();
            if (exampleTable == null)
                // this should never happen as the parser checks it already
                throw new GherkinSemanticErrorException(
                    "No examples defined in the example set.", position);

            return new ExampleSet(name, description, exampleTable);
        }

        public void ProcessTableRow(string[] row, FilePosition rowPosition)
        {
            tableBuilder.ProcessTableRow(row, rowPosition);
        }
    }
}