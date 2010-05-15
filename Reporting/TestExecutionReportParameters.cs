using System.IO;

namespace TechTalk.SpecFlow.Reporting
{
    public class TestExecutionReportParameters
    {
        private readonly string projectFile;
        private readonly string xmlTestResult;
        private readonly string labelledTestOutput;
        private readonly string outputFile;
        private readonly string xsltFile;

        public TestExecutionReportParameters(string projectFile, string xmlTestResult, string labelledTestOutput, string outputFile, string xsltFile)
        {
            this.projectFile = projectFile;
            this.xmlTestResult = Path.GetFullPath(xmlTestResult);
            this.labelledTestOutput = Path.GetFullPath(labelledTestOutput);
            this.outputFile = outputFile;
            this.xsltFile = xsltFile;
        }

        public string XsltFile
        {
            get { return xsltFile; }
        }

        public string OutputFile
        {
            get { return outputFile; }
        }

        public string LabelledTestOutput
        {
            get { return labelledTestOutput; }
        }

        public string XmlTestResult
        {
            get { return xmlTestResult; }
        }

        public string ProjectFile
        {
            get { return projectFile; }
        }
    }
}
