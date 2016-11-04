using System.Collections.Generic;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class ReportTableRow
    {
        public ReportTableRow()
        {
        }

        public ReportTableRow(List<ReportTableCell> cells)
        {
            Cells = cells;
        }

        public List<ReportTableCell> Cells { get; private set; }
    }
}