using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.NUnitExecutionReport.ReportElements
{
    public class ScenarioOutput
    {
        [XmlAttribute("name")]
        public string Name;
        public string Text;
    }
}