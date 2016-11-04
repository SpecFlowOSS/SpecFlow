using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    [XmlInclude(typeof(TableArgument))]
    [XmlInclude(typeof(DocStringArgument))]
    public abstract class ReportStepArgument
    {
        
    }
}