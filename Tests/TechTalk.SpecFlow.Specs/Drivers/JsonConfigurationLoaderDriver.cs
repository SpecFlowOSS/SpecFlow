using System.Collections.Generic;
using TechTalk.SpecFlow.TestProjectGenerator;
using TechTalk.SpecFlow.TestProjectGenerator.Driver;

namespace TechTalk.SpecFlow.Specs.Drivers
{
    public class JsonConfigurationLoaderDriver
    {
        private readonly ProjectsDriver _projectsDriver;
        private readonly ConfigurationDriver _configurationDriver;

        public JsonConfigurationLoaderDriver(ProjectsDriver projectsDriver, ConfigurationDriver configurationDriver)
        {
            _projectsDriver = projectsDriver;
            _configurationDriver = configurationDriver;
        }

        public void AddSpecFlowJson(string specFlowJson)
        {
            _configurationDriver.SetConfigurationFormat(ConfigurationFormat.Json);
            _projectsDriver.AddFile("specflow.json", specFlowJson, "None", new Dictionary<string, string> { ["CopyToOutputDirectory"] = "Always" });
        }
    }
}
