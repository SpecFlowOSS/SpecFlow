using System.Collections.Generic;
using System.Xml.Serialization;
using TechTalk.SpecFlow.Parser;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class Instance
    {
        [XmlAttribute("fromScenarioOutline")]
        public bool FromScenarioOutline;
        public FeatureRef FeatureRef;
        public ScenarioRef ScenarioRef;
        public List<Parameter> Parameters;
        public ReportStep ScenarioStep;
    }
}