using System.IO;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class SpecFlowFeatureFile
    {
        public string ProjectRelativePath { get; private set; }
        public string CustomNamespace { get; set; }
        public string GetFullPath(SpecFlowProject project)
        {
            return Path.GetFullPath(Path.Combine(project.ProjectFolder, ProjectRelativePath));
        }

        public SpecFlowFeatureFile(string path)
        {
            ProjectRelativePath = path;
        }

        public FeatureFileInput ToFeatureFileInput()
        {
            return new FeatureFileInput(ProjectRelativePath)
                       {
                           CustomNamespace = CustomNamespace
                       };
        }
    }
}