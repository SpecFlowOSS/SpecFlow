using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class ExampleBuilder : ITableProcessor
    {
        private readonly string text;
        private readonly FilePosition position;
        private Table exampleTable;

        public ExampleBuilder(string text, FilePosition position)
        {
            this.text = text;
            this.position = position;
        }

        public ExampleSet GetResult()
        {
            return new ExampleSet(new Text(text), exampleTable);
        }

        public void ProcessTable(Table table)
        {
            exampleTable = table;
        }
    }
}