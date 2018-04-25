using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class MsBuildProjectReader
    {
        public static SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFilePath)
        {
            var configurationProvider = new GeneratorConfigurationProvider(new ConfigurationLoader());
            var projectReader = new BuildalyzerProjectReader(configurationProvider);
            return projectReader.ReadSpecFlowProject(projectFilePath);
        }
    }
}
