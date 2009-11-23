using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using TechTalk.SpecFlow.Parser.Configuration;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport.ReportElements;

namespace TechTalk.SpecFlow.Reporting.NUnitExecutionReport
{
    internal class NUnitExecutionReportGenerator
    {
        private ReportElements.NUnitExecutionReport report;
        private SpecFlowProject specFlowProject;
        private readonly string xmlTestResultPath;
        private readonly string labeledTestOutputPath;

        public NUnitExecutionReportGenerator(SpecFlowProject specFlowProject, string xmlTestResultPath, string labeledTestOutputPath)
        {
            this.xmlTestResultPath = xmlTestResultPath;
            this.specFlowProject = specFlowProject;
            this.labeledTestOutputPath = labeledTestOutputPath;
        }

        public void GenerateReport()
        {
            report = new ReportElements.NUnitExecutionReport();
            report.ProjectName = specFlowProject.ProjectName;
            report.GeneratedAt = DateTime.Now.ToString("g", CultureInfo.InvariantCulture);

            XmlDocument xmlTestResult = LoadXmlTestResult();
            report.NUnitXmlTestResult = xmlTestResult.DocumentElement;

            if (File.Exists(labeledTestOutputPath))
            {
                using(var reader = new StreamReader(labeledTestOutputPath))
                {
                    string currentTest = "unknown";
                    List<string> testLines = new List<string>();

                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("***"))
                        {
                            CloseCurrentTest(testLines, currentTest);
                            currentTest = line.Trim('*', ' ');
                        }
                        else
                        {
                            testLines.Add(line);
                        }
                    }
                    CloseCurrentTest(testLines, currentTest);
                }
            }
        }

        private void CloseCurrentTest(List<string> testLines, string currentTest)
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

        private XmlDocument LoadXmlTestResult()
        {
            XmlDocument xmlTestResult = new XmlDocument();
            XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("", ReportElements.NUnitExecutionReport.XmlNUnitNamespace);
            using(XmlReader reader = XmlReader.Create(xmlTestResultPath, new XmlReaderSettings(), new XmlParserContext(null, nsmgr, null, XmlSpace.None)))
            {
                xmlTestResult.Load(reader);
            }
            return xmlTestResult;
        }

        public void TransformReport(string outputFilePath)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ReportElements.NUnitExecutionReport), ReportElements.NUnitExecutionReport.XmlNamespace);

            if (XsltHelper.IsXmlOutput(outputFilePath))
            {
                XsltHelper.TransformXml(serializer, report, outputFilePath);
            }
            else
            {
                XsltHelper.TransformHtml(serializer, report, GetType(), outputFilePath, specFlowProject.GeneratorConfiguration);
            }
        }
    }
}
