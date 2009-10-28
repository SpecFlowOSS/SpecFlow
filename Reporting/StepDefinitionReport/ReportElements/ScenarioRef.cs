using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class ScenarioRef
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("line")]
        public int SourceFileLine;
    }
}