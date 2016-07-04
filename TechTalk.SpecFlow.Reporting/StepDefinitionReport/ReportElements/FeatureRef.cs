using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class FeatureRef
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("file")]
        public string FilePath;
        
    }
}