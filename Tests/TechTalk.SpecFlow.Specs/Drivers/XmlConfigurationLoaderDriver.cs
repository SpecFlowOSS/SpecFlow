using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi.Driver;
using TechTalk.SpecFlow.TestProjectGenerator.NewApi._1_Memory;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class XmlConfigurationLoaderDriver
    {
        private readonly ConfigurationDriver _configurationDriver;
        private readonly SolutionDriver _solutionDriver;

        public XmlConfigurationLoaderDriver(ConfigurationDriver configurationDriver, SolutionDriver solutionDriver)
        {
            _configurationDriver = configurationDriver;
            _solutionDriver = solutionDriver;
        }

        public void AddFromXmlSpecFlowSection(string projectName, string specFlowSection)
        {
            var project = _solutionDriver.Projects[projectName];
            AddFromXmlSpecFlowSection(project, specFlowSection);
        }

        public void AddFromXmlSpecFlowSection(string specFlowSection)
        {
            AddFromXmlSpecFlowSection(_solutionDriver.DefaultProject, specFlowSection);
        }

        public void AddFromXmlSpecFlowSection(ProjectBuilder project, string specFlowSection, bool ignoreUnitTestProvider = true)
        {
            var configSection = ConfigurationSectionHandler.CreateFromXml(specFlowSection);
            var appConfigConfigurationLoader = new AppConfigConfigurationLoader();

            var specFlowConfiguration = appConfigConfigurationLoader.LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            foreach (string stepAssemblyName in specFlowConfiguration.AdditionalStepAssemblies)
            {
                _configurationDriver.AddStepAssembly(new StepAssembly(stepAssemblyName));
            }

            if (!ignoreUnitTestProvider)
            {
                _configurationDriver.SetUnitTestProvider(project, specFlowConfiguration.UnitTestProvider);
            }

            _configurationDriver.SetBindingCulture(project, specFlowConfiguration.BindingCulture);
            _configurationDriver.SetFeatureLanguage(project, specFlowConfiguration.FeatureLanguage);
        }
    }
}
