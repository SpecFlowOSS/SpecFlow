using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace TechTalk.SpecFlow.Reporting.TestExecutionReport
{
    public class TestExecutionReportGenerator
    {
        public void GenerateNUnitXmlFromGallio(string xmlTestResult)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            Stream rsrc = asm.GetManifestResourceStream("TechTalk.SpecFlow.Reporting.TestExecutionReport.Gallio2NUnit.xslt");
            XmlTextReader xmlTextReader = new XmlTextReader(rsrc);

            XDocument doc = XDocument.Load(xmlTestResult);
            var tranny = new XslCompiledTransform();
            tranny.Load(xmlTextReader);           
            XmlReader reader = doc.CreateReader();
            const string outputFileName = "TestResult.xml";
            XmlWriter writer = XmlWriter.Create(outputFileName);
            
            if (writer != null)
            {
                tranny.Transform(reader, writer);
                writer.Close();
            }
        }
    }
}
