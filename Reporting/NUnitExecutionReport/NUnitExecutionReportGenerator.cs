using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport.ReportElements;

namespace TechTalk.SpecFlow.Reporting.NUnitExecutionReport
{
    public class NUnitExecutionReportGenerator : NUnitBasedExecutionReportGenerator
    {
        private readonly NUnitExecutionReportParameters reportParameters;

        protected override ReportParameters ReportParameters
        {
            get { return reportParameters; }
        }

        public NUnitExecutionReportGenerator(NUnitExecutionReportParameters reportParameters)
        {            
            this.reportParameters = reportParameters;
        }

        protected override void ExtendReport(ReportElements.NUnitExecutionReport report)
        {
            if (File.Exists(reportParameters.LabelledTestOutput))
            {
                using(var reader = new StreamReader(reportParameters.LabelledTestOutput))
                {
                    string currentTest = "unknown";
                    List<string> testLines = new List<string>();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("***"))
                        {
                            CloseCurrentTest(testLines, currentTest, report);
                            currentTest = line.Trim('*', ' ');
                        }
                        else
                        {
                            testLines.Add(line);
                        }
                    }
                    CloseCurrentTest(testLines, currentTest, report);
                }
            }
        }

        private void CloseCurrentTest(List<string> testLines, string currentTest, ReportElements.NUnitExecutionReport report)
        {
            if (testLines.Count > 0)
            {
                report.ScenarioOutputs.Add(new ScenarioOutput
                                               {
                                                   Name = currentTest,
                                                   Text = string.Join(Environment.NewLine, testLines.ToArray())
                                               });
                testLines.Clear();
            }
        }

        protected override XmlDocument LoadXmlTestResult()
        {
            XmlDocument xmlTestResult = new XmlDocument();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("", ReportElements.NUnitExecutionReport.XmlNUnitNamespace);
            using(XmlReader reader = XmlReader.Create(reportParameters.XmlTestResult, new XmlReaderSettings(), new XmlParserContext(null, nsmgr, null, XmlSpace.None)))
            {
                xmlTestResult.Load(reader);
            }
            return xmlTestResult;
        }

    }
}
