using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class ReportTableCell
    {
        public ReportTableCell()
        {
        }

        public ReportTableCell(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}