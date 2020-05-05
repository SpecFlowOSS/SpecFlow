using System.Collections.Generic;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class SpecFlowProjectInfo
    {
        public SpecFlowProjectInfo(
            IReadOnlyCollection<GeneratorPluginInfo> generatorPlugins,
            IReadOnlyCollection<string> featureFiles,
            string projectPath,
            string projectFolder,
            string projectGuid,
            string projectAssemblyName,
            string outputPath,
            string rootNamespace,
            string targetFrameworks,
            string currentTargetFramework)
        {
            GeneratorPlugins = generatorPlugins;
            FeatureFiles = FileFilter.GetValidFiles(featureFiles);
            ProjectFolder = projectFolder;
            OutputPath = outputPath;
            RootNamespace = rootNamespace;
            TargetFrameworks = targetFrameworks;
            CurrentTargetFramework = currentTargetFramework;
            ProjectGuid = projectGuid;
            ProjectAssemblyName = projectAssemblyName;
            ProjectPath = projectPath;
        }

        public IReadOnlyCollection<GeneratorPluginInfo> GeneratorPlugins { get; }

        public IReadOnlyCollection<string> FeatureFiles { get; }

        public string ProjectPath { get; }

        public string ProjectFolder { get; }

        public string ProjectGuid { get; }

        public string ProjectAssemblyName { get; }

        public string OutputPath { get; }

        public string RootNamespace { get; }

        public string TargetFrameworks { get; }

        public string CurrentTargetFramework { get; }

    }
}
