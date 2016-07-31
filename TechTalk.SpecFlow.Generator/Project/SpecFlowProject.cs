using System;
using System.Collections.Generic;
using System.IO;
using TechTalk.SpecFlow.Generator.Configuration;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Utils;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class SpecFlowProject
    {
        public ProjectSettings ProjectSettings { get; set; }
        public SpecFlowProjectConfiguration Configuration { get; set; }
        public List<FeatureFileInput> FeatureFiles { get; private set; }

        public SpecFlowProject()
        {
            ProjectSettings = new ProjectSettings();
            FeatureFiles = new List<FeatureFileInput>();
            Configuration = new SpecFlowProjectConfiguration();
        }

        public FeatureFileInput GetOrCreateFeatureFile(string featureFilePath)
        {
            featureFilePath = Path.GetFullPath(Path.Combine(ProjectSettings.ProjectFolder, featureFilePath));
            var result = FeatureFiles.Find(ff => ff.GetFullPath(ProjectSettings).Equals(featureFilePath, StringComparison.InvariantCultureIgnoreCase)) ??
                         new FeatureFileInput(FileSystemHelper.GetRelativePath(featureFilePath, ProjectSettings.ProjectFolder));
            return result;
        }
    }
}