namespace TechTalk.SpecFlow.Generator.Project
{
    public interface IMSBuildProjectReader
    {
        SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFilePath, string rootNamespace);
    }
}
