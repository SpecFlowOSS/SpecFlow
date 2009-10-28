using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.NUnitExecutionReport.ReportElements
{
    [XmlType("NUnitExecutionReport", Namespace = XmlNamespace)]
    [XmlRoot("NUnitExecutionReport", Namespace = XmlNamespace)]
    public class NUnitExecutionReport
    {
        public const string XmlNamespace = "urn:TechTalk:SpecFlow.Report";
        public const string XmlNUnitNamespace = "urn:NUnit";

        [XmlAttribute("projectName")]
        public string ProjectName;
        [XmlAttribute("generatedAt")]
        public string GeneratedAt;

        [XmlElement(Namespace = XmlNamespace)]
        public XmlNode NUnitXmlTestResult;

        [XmlElement("ScenarioOutput")]
        public List<ScenarioOutput> ScenarioOutputs = new List<ScenarioOutput>();
    }
}
