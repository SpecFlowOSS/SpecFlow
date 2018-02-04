using System;
using System.Xml;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    [Serializable]
    public class SpecFlowConfigurationHolder : ISpecFlowConfigurationHolder
    {
        private readonly string xmlString;

        public ConfigSource ConfigSource { get; }

        public string Content => xmlString;

        public bool HasConfiguration => !string.IsNullOrEmpty(xmlString);

        public SpecFlowConfigurationHolder()
        {
            ConfigSource = ConfigSource.Default;
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