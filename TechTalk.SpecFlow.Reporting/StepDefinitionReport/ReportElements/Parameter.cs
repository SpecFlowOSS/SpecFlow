using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class Parameter
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlText]
        public string Value;
    }
}