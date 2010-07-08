using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Xsl;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport;
using TechTalk.SpecFlow.Reporting.NUnitExecutionReport.ReportElements;

namespace TechTalk.SpecFlow.Reporting.MsTestExecutionReport
{
    public class MsTestExecutionReportGenerator : NUnitBasedExecutionReportGenerator
    {
        private readonly MsTestExecutionReportParameters reportParameters;

        protected override ReportParameters ReportParameters
        {
            get { return reportParameters; }
        }

        protected override Type ReportType
        {
            get
            {
                return typeof (NUnitExecutionReportGenerator);
            }
        }

        public MsTestExecutionReportGenerator(MsTestExecutionReportParameters reportParameters)
        {
            this.reportParameters = reportParameters;
        }

        protected override XmlDocument LoadXmlTestResult()
        {
            //transform MsTest result to nunit and return

            XslCompiledTransform xslt = new XslCompiledTransform();

            using (var xsltReader = new ResourceXmlReader(typeof(MsTestExecutionReportGenerator), "MsTestToNUnit.xslt"))
            {
                var resourceResolver = new XmlResourceResolver();
                var xsltSettings = new XsltSettings(true, false);
                xslt.Load(xsltReader, xsltSettings, resourceResolver);
            }

            var writerStream = new MemoryStream();
            using (var xmlTextWriter = new XmlTextWriter(writerStream, Encoding.UTF8))
            {
                xslt.Transform(reportParameters.XmlTestResult, xmlTextWriter);
            }
            writerStream = new MemoryStream(writerStream.GetBuffer());
            XmlDocument result = new XmlDocument();
            result.Load(writerStream);
            return result;
        }

        protected override void ExtendReport(NUnitExecutionReport.ReportElements.NUnitExecutionReport report)
        {
            XmlDocument reportDoc = new XmlDocument();
            reportDoc.Load(reportParameters.XmlTestResult);

            XmlNameTable nameTable = new NameTable();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("mstest", "http://microsoft.com/schemas/VisualStudio/TeamTest/2006");
            var testResultNodes = reportDoc.SelectNodes("//mstest:UnitTestResult", namespaceManager);
            if (testResultNodes == null)
                return;

            foreach (XmlElement testResultNode in testResultNodes)
            {
                var testIdAttr = testResultNode.Attributes["testId"];
                if (testIdAttr == null)
                    continue;

                var testMethodElement = reportDoc.SelectSingleNode(
                    string.Format(
                        "//mstest:UnitTest[@id = '{0}']/mstest:TestMethod", testIdAttr.Value), namespaceManager)
                                        as XmlElement;

                if (testMethodElement == null)
                    continue;

                var classNameAttr = testMethodElement.Attributes["className"];
                if (classNameAttr == null)
                    continue;
                string testClassName = classNameAttr.Value.Split(new[] {','}, 2)[0].Trim();

                var nameAttr = testMethodElement.Attributes["name"];
                if (nameAttr == null)
                    continue;

                var stdOut = testResultNode.SelectSingleNode("mstest:Output/mstest:StdOut", namespaceManager);
                if (stdOut == null)
                    continue;

                report.ScenarioOutputs.Add(
                    new ScenarioOutput()
                        {
                            Name = testClassName + "." + nameAttr.Value,
                            Text = stdOut.InnerText
                        }
                    );
            }
        }
    }
}
