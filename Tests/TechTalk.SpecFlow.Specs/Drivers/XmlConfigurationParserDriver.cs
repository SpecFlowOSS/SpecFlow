using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class XmlConfigurationParserDriver
    {
        private readonly AppConfigConfigurationLoader _appConfigConfigurationLoader;

        public XmlConfigurationParserDriver(AppConfigConfigurationLoader appConfigConfigurationLoader)
        {
            _appConfigConfigurationLoader = appConfigConfigurationLoader;
        }

        public SpecFlowConfiguration ParseSpecFlowSection(string specFlowSection)
        {
            var configSection = ConfigurationSectionHandler.CreateFromXml(specFlowSection);

            var specFlowConfiguration = _appConfigConfigurationLoader.LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);
            return specFlowConfiguration;
        }
    }
}
