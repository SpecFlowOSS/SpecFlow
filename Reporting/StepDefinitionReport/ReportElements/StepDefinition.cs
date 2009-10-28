using System.Collections.Generic;
using System.Xml.Serialization;
using TechTalk.SpecFlow.Parser.SyntaxElements;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class StepDefinition
    {
        [XmlAttribute("type")]
        public string Type;

        public Binding Binding;
        public ScenarioStep ScenarioStep;
        public List<Instance> Instances = new List<Instance>();
    }
}