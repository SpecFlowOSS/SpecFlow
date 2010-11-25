using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public static class RuntimeConfigurationReader
    {
        public static void UpdateConfigFromFile(RuntimeConfigurationForGenerator runtimeConfiguration, string configFile)
        {
            using (TextReader file = new StreamReader(configFile))
            {
                UpdateConfigFromFileContent(runtimeConfiguration, file.ReadToEnd());
            }
        }

        public static void UpdateConfigFromFileContent(RuntimeConfigurationForGenerator runtimeConfiguration, string configFileContent)
        {
            XmlDocument configDocument;
            try
            {
                configDocument = new XmlDocument();
                configDocument.LoadXml(configFileContent);
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex, "Config load error");
                return;
            }

            var specFlowNode = configDocument.SelectSingleNode("/configuration/specFlow");
            if (specFlowNode == null)
                return;

            var section = ConfigurationSectionHandler.CreateFromXml(specFlowNode);
            runtimeConfiguration.UpdateFromConfigFile(section);
        }
    }
}