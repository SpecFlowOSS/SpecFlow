using System.Collections.Generic;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow.Generator;

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
            FeatureFiles = RemoveFilesWithInvalidChars(featureFiles);
            //FeatureFiles = featureFiles;
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

        private IReadOnlyCollection<string> RemoveFilesWithInvalidChars(IEnumerable<string> featureFilePaths)
        {
            return featureFilePaths.Where(p => !InvalidFileName(p)).ToList();
        }

        private bool InvalidFileName(string featureFilePath)
        {
            var featureFileName = Path.GetFileName(featureFilePath);
            var invalidCharacters = Path.GetInvalidFileNameChars();

            return !string.IsNullOrEmpty(featureFileName) &&
                   featureFileName.Any(s => invalidCharacters.Contains(s));
        }

        public override string ToString()
        {
            return $"GeneratorPlugins: '{string.Join(", ", GeneratorPlugins)}', "
                   + $"FeatureFiles: '{string.Join(", ", FeatureFiles)}', "
                   + $"ProjectPath: '{ProjectPath}', "
                   + $"ProjectFolder: '{ProjectFolder}', "
                   + $"ProjectGuid: '{ProjectGuid}', "
                   + $"ProjectAssemblyName: '{ProjectAssemblyName}', "
                   + $"OutputPath: '{OutputPath}', "
                   + $"RootNamespace: '{RootNamespace}', "
                   + $"TargetFrameworks: '{TargetFrameworks}', "
                   + $"CurrentTargetFramework: '{CurrentTargetFramework}', "
                   ;
        }
    }
}
