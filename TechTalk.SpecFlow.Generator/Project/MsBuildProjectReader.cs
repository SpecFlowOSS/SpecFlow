namespace TechTalk.SpecFlow.Generator.Project
{
    public class MSBuildProjectReader : IMSBuildProjectReader
    {
        private readonly ISpecFlowProjectReader _projectReader;

        public MSBuildProjectReader(ISpecFlowProjectReader projectReader)
        {
            _projectReader = projectReader;
        }

        public SpecFlowProject LoadSpecFlowProjectFromMsBuild(string projectFilePath, string rootNamespace)
        {
            return _projectReader.ReadSpecFlowProject(projectFilePath, rootNamespace);
        }
    }
}
