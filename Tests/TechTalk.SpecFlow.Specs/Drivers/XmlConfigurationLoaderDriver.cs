using SpecFlow.TestProjectGenerator.NewApi.Driver;
using SpecFlow.TestProjectGenerator.NewApi._1_Memory;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Configuration.AppConfig;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class XmlConfigurationLoaderDriver
    {
        private readonly ConfigurationDriver _configurationDriver;
        private readonly ProjectsDriver _projectsDriver;

        public XmlConfigurationLoaderDriver(ConfigurationDriver configurationDriver, ProjectsDriver projectsDriver)
        {
            _configurationDriver = configurationDriver;
            _projectsDriver = projectsDriver;
        }
        public void AddFromXmlSpecFlowSection(string projectName, string specFlowSection)
        {
            var project = _projectsDriver.Projects[projectName];
            AddFromXmlSpecFlowSection(project, specFlowSection);
        }

        public void AddFromXmlSpecFlowSection(string specFlowSection)
        {
            AddFromXmlSpecFlowSection(_projectsDriver.DefaultProject, specFlowSection);
        }

        public void AddFromXmlSpecFlowSection(ProjectBuilder project, string specFlowSection)
        {
            var configSection = ConfigurationSectionHandler.CreateFromXml(specFlowSection);
            var appConfigConfigurationLoader = new AppConfigConfigurationLoader();

            var specFlowConfiguration = appConfigConfigurationLoader.LoadAppConfig(ConfigurationLoader.GetDefault(), configSection);

            foreach (string stepAssemblyName in specFlowConfiguration.AdditionalStepAssemblies)
            {
                _configurationDriver.AddStepAssembly(new StepAssembly(stepAssemblyName));
            }

            _configurationDriver.SetUnitTestProvider(project, specFlowConfiguration.UnitTestProvider);
            _configurationDriver.SetBindingCulture(project, specFlowConfiguration.BindingCulture);
        }
    }
}
