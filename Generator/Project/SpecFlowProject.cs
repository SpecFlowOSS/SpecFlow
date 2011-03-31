using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class SpecFlowProject
    {
        public ProjectSettings ProjectSettings { get; set; }
        public SpecFlowConfigurationHolder ConfigurationHolder { get; set; }
        public SpecFlowProjectConfiguration Configuration { get; set; }
        public List<SpecFlowFeatureFile> FeatureFiles { get; private set; }

        public string ProjectName { get; set; }
        public string AssemblyName { get; set; }
        public string ProjectFolder { get; set; }
        public string DefaultNamespace { get; set; }
        public GeneratorConfiguration GeneratorConfiguration
        {
            get { return Configuration.GeneratorConfiguration; }
        }

        public SpecFlowProject()
        {
            FeatureFiles = new List<SpecFlowFeatureFile>();
            Configuration = new SpecFlowProjectConfiguration();
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