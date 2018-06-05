using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class MsBuildProjectReader
    {
        public static SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFilePath, string rootNamespace)
        {
            var configurationProvider = new GeneratorConfigurationProvider(new ConfigurationLoader());
            var projectReader = new ProjectReader(configurationProvider, new ProjectLanguageReader());
            return projectReader.ReadSpecFlowProject(projectFilePath, rootNamespace);
        }
    }
}
