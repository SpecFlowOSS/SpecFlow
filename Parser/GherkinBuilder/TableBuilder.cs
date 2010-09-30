using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class TableBuilder : ITableProcessor
    {
        private readonly List<Row> tableRows = new List<Row>();

        public Table GetResult()
        {
            return tableRows.Count == 0 ? null :
                new Table(tableRows[0], tableRows.Skip(1).ToArray());
        }

        public void ProcessTableRow(string[] cells, FilePosition rowPosition)
        {
            var row = new Row(cells.Select(c => new Cell(c)).ToArray());
            row.FilePosition = rowPosition;

            if (tableRows.Count > 0 && tableRows[0].Cells.Length != row.Cells.Length)
            {
                throw new GherkinSemanticErrorException(
                    "Number of cells in the row does not match the number of cells in the header.", row.FilePosition);
            }

            tableRows.Add(row);
        }
    }
}