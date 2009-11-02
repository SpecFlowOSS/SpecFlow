using System.Collections.Generic;
using System.IO;

namespace TechTalk.SpecFlow.Parser.Configuration
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
    }

    public class SpecFlowProject
    {
        public string ProjectName { get; set; }
        public string AssemblyName { get; set; }
        public string ProjectFolder { get; set; }
        public string DefaultNamespace { get; set; }
        public List<SpecFlowFeatureFile> FeatureFiles { get; private set; }
        public GeneratorConfiguration GeneratorConfiguration { get; set; }

        public SpecFlowProject()
        {
            FeatureFiles = new List<SpecFlowFeatureFile>();
            GeneratorConfiguration = new GeneratorConfiguration(); // load defaults
        }
    }
}