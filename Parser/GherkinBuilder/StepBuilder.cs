using System;
using System.Collections.Generic;
using System.Linq;
using gherkin;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class StepBuilder : ITableProcessor
    {
        private readonly ScenarioStep step;
        private readonly List<Row> tableRows = new List<Row>();

        public StepBuilder(string keyword, string text, FilePosition position, I18n i18n)
        {
            if (i18n.keywords("given").contains(keyword)) step = new Given();
            else if (i18n.keywords("when").contains(keyword)) step = new When();
            else if (i18n.keywords("then").contains(keyword)) step = new Then();
            else if (i18n.keywords("and").contains(keyword)) step = new And();
            else if (i18n.keywords("but").contains(keyword)) step = new But();
            else throw new ArgumentOutOfRangeException(string.Format("Parameter 'keyword' has value that can not be translated! Value:'{0}'", keyword));

            step.Text = text;
            step.FilePosition = position;
        }

        public ScenarioStep GetResult()
        {
            step.TableArg = tableRows.Count == 0 ? null : 
                new Table(tableRows[0], tableRows.Skip(1).ToArray());

            return step;
        }

        public void SetMultilineArg(string text)
        {
            step.MultiLineTextArgument = text;
        }

        public void ProcessTableRow(string[] cells, int lineNumber)
        {
            var row = new Row(cells.Select(c => new Cell(c)).ToArray());
            row.FilePosition = new FilePosition(lineNumber, 1);
            tableRows.Add(row);
        }
    }
}