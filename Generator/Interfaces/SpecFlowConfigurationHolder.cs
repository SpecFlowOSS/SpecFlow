using System;
using System.Xml;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class SpecFlowConfigurationHolder
    {
        private readonly string xmlString;

        public string XmlString
        {
            get { return xmlString; }
        }

        public bool HasConfiguration
        {
            get { return !string.IsNullOrEmpty(xmlString); }
        }

        public SpecFlowConfigurationHolder()
        {
            this.xmlString = null;
        }

        public SpecFlowConfigurationHolder(string configXml)
        {
            this.xmlString = configXml;
        }

        public SpecFlowConfigurationHolder(XmlNode configXmlNode)
        {
            this.xmlString = configXmlNode == null ? null : configXmlNode.OuterXml;
        }
    }
}