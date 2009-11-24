using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType("StepDefinitionReport", Namespace = XmlNamespace)]
    [XmlRoot("StepDefinitionReport", Namespace = XmlNamespace)]
    public class StepDefinitionReport
    {
        public const string XmlNamespace = "urn:TechTalk:SpecFlow.Report";

        [XmlAttribute("projectName")]
        public string ProjectName;
        [XmlAttribute("generatedAt")]
        public string GeneratedAt;
        [XmlAttribute("showBindingsWithoutInsance")]
        public bool ShowBindingsWithoutInsance;

        [XmlElement("StepDefinition")]
        public List<StepDefinition> StepDefinitions = new List<StepDefinition>();
    }
}