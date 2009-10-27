using System;
using System.Linq;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    public class Cell
    {
        public string Value { get; set; }

        public Cell()
        {
        }

        public Cell(Text value)
        {
            Value = value.Value;
        }
    }

    public class Row
    {
        public Cell[] Cells { get; set; }

        public Row()
        {
        }

        public Row(params Cell[] cells)
        {
            Cells = cells;
        }
    }

    public class Table
    {
        public Row Header { get; set; }
        public Row[] Body { get; set; }

        public Table()
        {
        }

        public Table(Row header, params Row[] body)
        {
            Header = header;
            Body = body;
        }
    }
}