using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow.Configuration;

namespace TechTalk.SpecFlow.Generator.Configuration
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

    public class SpecFlowProjectConfiguration
    {
        public GeneratorConfiguration GeneratorConfiguration { get; set; }
        public RuntimeConfigurationForGenerator RuntimeConfiguration { get; set; }

        public SpecFlowProjectConfiguration()
        {
            GeneratorConfiguration = new GeneratorConfiguration(); // load defaults
            RuntimeConfiguration = new RuntimeConfigurationForGenerator(); // load defaults
        }

        #region Equality
        public bool Equals(SpecFlowProjectConfiguration other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other.GeneratorConfiguration, GeneratorConfiguration) && Equals(other.RuntimeConfiguration, RuntimeConfiguration);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (SpecFlowProjectConfiguration)) return false;
            return Equals((SpecFlowProjectConfiguration) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((GeneratorConfiguration != null ? GeneratorConfiguration.GetHashCode() : 0)*397) ^ (RuntimeConfiguration != null ? RuntimeConfiguration.GetHashCode() : 0);
            }
        }
        #endregion
    }

    public class SpecFlowProject
    {
        public string ProjectName { get; set; }
        public string AssemblyName { get; set; }
        public string ProjectFolder { get; set; }
        public string DefaultNamespace { get; set; }
        public List<SpecFlowFeatureFile> FeatureFiles { get; private set; }
        public GeneratorConfiguration GeneratorConfiguration
        {
            get { return Configuration.GeneratorConfiguration; }
        }
        public SpecFlowProjectConfiguration Configuration { get; set; }

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