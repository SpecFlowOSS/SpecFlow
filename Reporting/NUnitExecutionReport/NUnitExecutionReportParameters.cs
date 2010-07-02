using System.IO;

namespace TechTalk.SpecFlow.Reporting.NUnitExecutionReport
{
    public class NUnitExecutionReportParameters : ReportParameters
    {
        public string LabelledTestOutput { get; private set; }
        public string XmlTestResult { get; private set; }

        public NUnitExecutionReportParameters(string projectFile, string xmlTestResult, string labelledTestOutput, string outputFile, string xsltFile)
            : base(projectFile, outputFile, xsltFile)
        {
            this.XmlTestResult = Path.GetFullPath(xmlTestResult);
            this.LabelledTestOutput = string.IsNullOrEmpty(labelledTestOutput) ? "" : Path.GetFullPath(labelledTestOutput);
        }
    }
}