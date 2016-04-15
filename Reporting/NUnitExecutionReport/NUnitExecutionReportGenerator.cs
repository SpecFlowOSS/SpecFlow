using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
                var allLines = File.ReadAllLines(reportParameters.LabelledTestOutput);
                var TestNameLines = allLines.Where(line => line.StartsWith("=>"));
                var testNames = TestNameLines.Select(line => line.Remove(0, 3));
                foreach(var test in testNames)
                {
                    var scenarioLines = allLines.Where(line => !line.StartsWith("=>") && line.Contains($"[{test}]"));
                    List<string> testLines = new List<string>();
                    foreach (var line in scenarioLines)
                    {
                        string testLine = string.Empty;
                        if(line.StartsWith("#"))
                        {
                            testLine = line.Remove(0, test.Length + "#[]: ".Length);
                            testLines.Add(testLine);
                        }
                        else if(line.StartsWith("->"))
                        {
                            testLine = "->" + line.Remove(0, test.Length + "->#[]: ".Length);
                            testLines.Add(testLine);
                        }
                    }
                    CloseCurrentTest(testLines, test, report);

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
