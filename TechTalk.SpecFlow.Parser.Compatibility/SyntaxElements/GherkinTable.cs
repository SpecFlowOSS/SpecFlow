using System;
using System.Linq;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Parser.SyntaxElements
{
    [XmlType("Cell")]
    public class GherkinTableCell
    {
        public string Value { get; set; }
        public FilePosition FilePosition { get; set; }

        public GherkinTableCell()
        {
        }

        public GherkinTableCell(string value)
        {
            Value = value;
        }
    }

    [XmlType("Row")]
    public class GherkinTableRow
    {
        public GherkinTableCell[] Cells { get; set; }
        public FilePosition FilePosition { get; set; }

        public GherkinTableRow()
        {
        }

        public GherkinTableRow(params GherkinTableCell[] cells)
        {
            Cells = cells;
        }
    }

    [XmlType("Table")]
    public class GherkinTable
    {
        public GherkinTableRow Header { get; set; }
        public GherkinTableRow[] Body { get; set; }

        public GherkinTable()
        {
        }

        public GherkinTable(GherkinTableRow header, params GherkinTableRow[] body)
        {
            Header = header;
            Body = body;
        }
    }
}