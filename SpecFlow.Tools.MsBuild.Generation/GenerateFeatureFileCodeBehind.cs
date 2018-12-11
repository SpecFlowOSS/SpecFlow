using System.Collections.Generic;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IGenerateFeatureFileCodeBehind
    {
        IEnumerable<string> GenerateFilesForProject(List<string> generatorPlugins, string projectPath, string projectFolder, string outputPath, string rootNamespace, List<string> featureFiles);
    }

    public class GenerateFeatureFileCodeBehind : IGenerateFeatureFileCodeBehind
    {
        public IEnumerable<string> GenerateFilesForProject(List<string> generatorPlugins, string projectPath, string projectFolder, string outputPath, string rootNamespace, List<string> featureFiles)
        {
            var generator = new FeatureFileCodeBehindGenerator();
            return generator.GenerateFilesForProject(projectPath, rootNamespace, featureFiles, generatorPlugins, projectFolder, outputPath);
        }
    }
}