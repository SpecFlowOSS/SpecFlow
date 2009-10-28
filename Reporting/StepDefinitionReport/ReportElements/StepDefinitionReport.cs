using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using TechTalk.SpecFlow.Parser.SyntaxElements;

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

        [XmlElement("StepDefinition")]
        public List<StepDefinition> StepDefinitions = new List<StepDefinition>();
    }

    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class StepDefinition
    {
        [XmlAttribute("type")]
        public string Type;

        public Binding Binding;
        public ScenarioStep ScenarioStep;
        public List<Instance> Instances = new List<Instance>();
    }

    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class Binding
    {
        public string MethodReference;
    }

    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class Instance
    {
        [XmlAttribute("fromScenarioOutline")]
        public bool FromScenarioOutline;
        public FeatureRef FeatureRef;
        public ScenarioRef ScenarioRef;
        public List<Parameter> Parameters;
        public ScenarioStep ScenarioStep;
    }

    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class FeatureRef
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("file")]
        public string FilePath;
        
    }

    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class ScenarioRef
    {
        [XmlAttribute("name")]
        public string Name;
    }

    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class Parameter
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlText]
        public string Value;
    }
}