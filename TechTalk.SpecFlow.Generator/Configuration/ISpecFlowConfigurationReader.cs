using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public interface ISpecFlowConfigurationReader
    {
        SpecFlowConfigurationHolder ReadConfiguration();
    }
}