using System;
using System.Xml;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    [Serializable]
    public class SpecFlowConfigurationHolder : ISpecFlowConfigurationHolder
    {
        private readonly string _content;

        public ConfigSource ConfigSource { get; private set; }

        public string Content => _content;

        public bool HasConfiguration => !string.IsNullOrEmpty(_content);

        public SpecFlowConfigurationHolder()
        {
            _content = null;
        }

        public SpecFlowConfigurationHolder(ConfigSource configSource, string content)
        {
            ConfigSource = configSource;
            _content = content;
        }

        public SpecFlowConfigurationHolder(XmlNode configXmlNode)
        {
            _content = configXmlNode?.OuterXml;
            ConfigSource = ConfigSource.AppConfig;
        }
    }

}