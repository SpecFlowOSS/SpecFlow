using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Xsl;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Reporting
{
    internal static class XsltHelper
    {
        public static bool IsXmlOutput(string outputFilePath)
        {
            return Path.GetExtension(outputFilePath).Equals(".xml", StringComparison.InvariantCultureIgnoreCase);
        }

        public static void TransformXml(XmlSerializer serializer, object report, string outputFilePath)
        {
            string xmlOutputPath = Path.ChangeExtension(outputFilePath, ".xml");

            using (var writer = new StreamWriter(xmlOutputPath, false, Encoding.UTF8))
            {
                serializer.Serialize(writer, report);
            }
        }

        public static void TransformHtml(XmlSerializer serializer, object report, Type reportType, string outputFilePath, Configuration.SpecFlowConfiguration generatorSpecFlowConfiguration, string xsltFile)
        {
            var xmlOutputWriter = new StringWriter();
            serializer.Serialize(xmlOutputWriter, report);

            XslCompiledTransform xslt = new XslCompiledTransform();
	    var allowXsltScripts = !string.IsNullOrEmpty(xsltFile);
            var xsltSettings = new XsltSettings(true, allowXsltScripts);
            XmlResolver resourceResolver;

            var reportName = reportType.Name.Replace("Generator", "");
            using (var xsltReader = GetTemplateReader(reportType, reportName, xsltFile))
            {
                resourceResolver = new XmlResourceResolver();
                xslt.Load(xsltReader, xsltSettings, resourceResolver);
            }

            var xmlOutputReader = new XmlTextReader(new StringReader(xmlOutputWriter.ToString()));

            XsltArgumentList argumentList = new XsltArgumentList();
            argumentList.AddParam("feature-language", "", generatorSpecFlowConfiguration.FeatureLanguage.Name);
            
            using (var xmlTextWriter = new XmlTextWriter(outputFilePath, Encoding.UTF8))
            {
				xslt.Transform(xmlOutputReader, argumentList, xmlTextWriter, resourceResolver);
            }            
        }

        private static XmlReader GetTemplateReader(Type reportType, string reportName, string xsltFile)
        {
            if (string.IsNullOrEmpty(xsltFile))
                return new ResourceXmlReader(reportType, reportName + ".xslt");

            return new XmlTextReader(xsltFile);
        }
    }
}
