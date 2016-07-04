using System;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Reporting.MsTestExecutionReport
{
    public class MsTestExecutionReportParameters : ReportParameters
    {
        public string XmlTestResult { get; private set; }

        public MsTestExecutionReportParameters(string projectFile, string xmlTestResult, string outputFile, string xsltFile)
            : base(projectFile, outputFile, xsltFile)
        {
            this.XmlTestResult = Path.GetFullPath(xmlTestResult);
        }
    }
}
