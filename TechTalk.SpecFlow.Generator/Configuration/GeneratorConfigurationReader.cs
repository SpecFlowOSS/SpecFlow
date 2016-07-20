using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    [Obsolete]
    public static class GeneratorConfigurationReader
    {
        public static void UpdateConfigFromFile(GeneratorConfiguration generatorConfiguration, string configFile)
        {
            using (TextReader file = new StreamReader(configFile))
            {
                UpdateConfigFromFileContent(generatorConfiguration, file.ReadToEnd());
            }
        }

        public static void UpdateConfigFromFileContent(GeneratorConfiguration generatorConfiguration, string configFileContent)
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
            generatorConfiguration.UpdateFromConfigFile(section);
        }
    }
}