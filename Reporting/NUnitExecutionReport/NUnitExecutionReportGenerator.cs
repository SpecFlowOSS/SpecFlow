using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport.ReportElements;

namespace TechTalk.SpecFlow.Reporting.NUnitExecutionReport
{
    public class NUnitExecutionReportGenerator
    {
        private ReportElements.NUnitExecutionReport report;
        private readonly SpecFlowProject specFlowProject;
        private readonly string xmlTestResultPath;
        private readonly string labeledTestOutputPath;

        public NUnitExecutionReportGenerator(string projectFile, string xmlTestResultPath, string labeledTestOutputPath)
        {
            this.xmlTestResultPath = xmlTestResultPath;
            specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(projectFile);
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

        public void TransformReport(string outputFilePath, string xsltFile)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ReportElements.NUnitExecutionReport), ReportElements.NUnitExecutionReport.XmlNamespace);

            if (XsltHelper.IsXmlOutput(outputFilePath))
            {
                XsltHelper.TransformXml(serializer, report, outputFilePath);
            }
            else
            {
                XsltHelper.TransformHtml(serializer, report, GetType(), outputFilePath, specFlowProject.GeneratorConfiguration, xsltFile);
            }
        }
    }
}
