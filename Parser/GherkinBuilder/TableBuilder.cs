using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Parser.GherkinBuilder
{
    internal class TableBuilder : ITableProcessor
    {
        private readonly List<GherkinTableRow> tableRows = new List<GherkinTableRow>();

        public GherkinTable GetResult()
        {
            return tableRows.Count == 0 ? null :
                new GherkinTable(tableRows[0], tableRows.Skip(1).ToArray());
        }

        public void ProcessTableRow(string[] cells, FilePosition rowPosition)
        {
            var row = new GherkinTableRow(cells.Select(c => new GherkinTableCell(c)).ToArray());
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