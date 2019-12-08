using TechTalk.SpecFlow.Generator.Project;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface ISpecFlowProjectProvider
    {
        SpecFlowProject GetSpecFlowProject();
    }
}
