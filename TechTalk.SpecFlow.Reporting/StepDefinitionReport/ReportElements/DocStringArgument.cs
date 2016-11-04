using System.Xml.Serialization;

namespace TechTalk.SpecFlow.Reporting.StepDefinitionReport.ReportElements
{
    [XmlType(Namespace = StepDefinitionReport.XmlNamespace)]
    public class DocStringArgument : ReportStepArgument
    {
        public DocStringArgument(string contentType, string content)
        {
            ContentType = contentType;
            Content = content;
        }

        public DocStringArgument()
        {
        }

        public string ContentType { get; set; }

        public string Content { get; set; }
    }
}