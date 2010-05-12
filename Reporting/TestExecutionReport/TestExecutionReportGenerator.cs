using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace TechTalk.SpecFlow.Reporting.TestExecutionReport
{
    public class TestExecutionReportGenerator
    {
        public void GenerateNUnitXmlFromGallio(string xmlTestResult)
        {
            using (XmlTextReader xmlTextReader = new ResourceXmlReader(Assembly.GetExecutingAssembly(),
                "TechTalk.SpecFlow.Reporting.TestExecutionReport.Gallio2NUnit.xslt"))
            {
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
}
