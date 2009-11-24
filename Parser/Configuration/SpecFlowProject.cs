using System;
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

        public SpecFlowFeatureFile GetOrCreateFeatureFile(string featureFileName)
        {
            featureFileName = Path.GetFullPath(Path.Combine(ProjectFolder, featureFileName));
            var result = FeatureFiles.Find(ff => ff.GetFullPath(this).Equals(featureFileName, StringComparison.InvariantCultureIgnoreCase));
            if (result == null)
            {
                result = new SpecFlowFeatureFile(featureFileName); //TODO: make it project relative
            }
            return result;
        }
    }
}