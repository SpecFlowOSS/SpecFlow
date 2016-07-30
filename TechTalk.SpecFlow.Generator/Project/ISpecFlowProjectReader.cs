using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Project
{
    public interface ISpecFlowProjectReader
    {
        SpecFlowProject ReadSpecFlowProject(string projectFilePath);
    }
}