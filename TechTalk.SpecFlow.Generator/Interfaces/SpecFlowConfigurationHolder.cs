using System;
using System.Xml;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class SpecFlowConfigurationHolder : ISpecFlowConfigurationHolder
    {
        private readonly string xmlString;

        public ConfigSource ConfigSource { get; private set; }

        public string Content => xmlString;

        public bool HasConfiguration => !string.IsNullOrEmpty(xmlString);

        public SpecFlowConfigurationHolder()
        {
            xmlString = null;
        }

        public SpecFlowConfigurationHolder(ConfigSource configSource, string content)
        {
            ConfigSource = configSource;
            xmlString = content;
        }

        public SpecFlowConfigurationHolder(XmlNode configXmlNode)
        {
            xmlString = configXmlNode?.OuterXml;
            ConfigSource = ConfigSource.AppConfig;
        }
    }

}