using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.TestProjectGenerator.Data;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class ConfigurationLoaderDriver
    {
        private readonly ConfigurationDriver _configurationDriver;
        private readonly SolutionDriver _solutionDriver;

        public ConfigurationLoaderDriver(ConfigurationDriver configurationDriver, SolutionDriver solutionDriver)
        {
            _configurationDriver = configurationDriver;
            _solutionDriver = solutionDriver;
        }

        public void SetFromSpecFlowConfiguration(SpecFlowConfiguration specFlowConfiguration)
        {
            var project = _solutionDriver.DefaultProject;

            foreach (string stepAssemblyName in specFlowConfiguration.AdditionalStepAssemblies)
            {
                _configurationDriver.AddStepAssembly(project, new StepAssembly(stepAssemblyName));
            }

            _configurationDriver.SetBindingCulture(project, specFlowConfiguration.BindingCulture);
            _configurationDriver.SetFeatureLanguage(project, specFlowConfiguration.FeatureLanguage);
        }
    }
}
