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
        private XmlDocument xmlTestResultDoc;
        private string namespaceURI;

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

        private class NamespaceReplacerTable : NameTable
        {
            private readonly string originalNamespace;
            private readonly string newNamespace;

            public NamespaceReplacerTable(string originalNamespace, string newNamespace)
            {
                this.originalNamespace = originalNamespace;
                this.newNamespace = newNamespace;
            }

            public override string Add(string key)
            {
                if (string.Equals(key, originalNamespace, StringComparison.InvariantCultureIgnoreCase))
                    return base.Add(newNamespace);
                return base.Add(key);
            }

            public override string Add(char[] key, int start, int len)
            {
                return Add(new string(key, start, len));
            }
        }

        private NamespaceReplacerTable GetNameTable()
        {
            return new NamespaceReplacerTable(
                "http://microsoft.com/schemas/VisualStudio/TeamTest/2006",
                namespaceURI);
        }

        protected override XmlDocument LoadXmlTestResult()
        {
            //transform MsTest result to nunit and return
            xmlTestResultDoc = new XmlDocument();
            xmlTestResultDoc.Load(reportParameters.XmlTestResult);
            namespaceURI = xmlTestResultDoc.SelectSingleNode("/*").NamespaceURI;

            var nameTable = GetNameTable();

            XslCompiledTransform xslt = new XslCompiledTransform();

            using (var xsltReader = new ResourceXmlReader(typeof(MsTestExecutionReportGenerator), "MsTestToNUnit.xslt", nameTable))
            {
                var resourceResolver = new XmlResourceResolver();
                var xsltSettings = new XsltSettings(true, false);
                xslt.Load(xsltReader, xsltSettings, resourceResolver);
            }

            var writerStream = new MemoryStream();
            using (var xmlTextWriter = new XmlTextWriter(writerStream, Encoding.UTF8))
            {
                xslt.Transform(xmlTestResultDoc, xmlTextWriter);
            }
            writerStream = new MemoryStream(writerStream.GetBuffer());
            XmlDocument result = new XmlDocument();
            result.Load(writerStream);
            return result;
        }

        protected override void ExtendReport(NUnitExecutionReport.ReportElements.NUnitExecutionReport report)
        {
            XmlNameTable nameTable = new NameTable();
            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
            namespaceManager.AddNamespace("mstest", namespaceURI);
            var testResultNodes = xmlTestResultDoc.SelectNodes("//mstest:UnitTestResult", namespaceManager);
            if (testResultNodes == null)
                return;

            foreach (XmlElement testResultNode in testResultNodes)
            {
                var testIdAttr = testResultNode.Attributes["testId"];
                if (testIdAttr == null)
                    continue;

                var testMethodElement = xmlTestResultDoc.SelectSingleNode(
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
