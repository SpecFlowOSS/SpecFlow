using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public interface ISpecFlowProjectConfigurationLoader
    {
        SpecFlowProjectConfiguration LoadConfiguration(SpecFlowConfigurationHolder configurationHolder, IProjectReference projectReference);
    }
}
