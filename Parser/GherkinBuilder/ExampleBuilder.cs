using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ExampleBuilder : ITableProcessor
    {
        private readonly Tags tags;
        private readonly string keyword;
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

        public ExampleBuilder(string keyword, string name, string description, Tags tags, FilePosition position)
        {
            this.keyword = keyword;
            this.name = name;
            this.description = description;
            this.tags = tags;
            this.position = position;
        }

        public ExampleSet GetResult()
        {
            GherkinTable exampleTable = tableBuilder.GetResult();
            if (exampleTable == null)
                // this should never happen as the parser checks it already
                throw new GherkinSemanticErrorException(
                    "No examples defined in the example set.", position);

            return new ExampleSet(keyword, name, description, tags, exampleTable);
        }

        public void ProcessTableRow(string[] row, FilePosition rowPosition)
        {
            tableBuilder.ProcessTableRow(row, rowPosition);
        }
    }
}