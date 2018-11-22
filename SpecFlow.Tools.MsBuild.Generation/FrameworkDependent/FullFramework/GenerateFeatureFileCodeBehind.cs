using System.Collections.Generic;

namespace SpecFlow.Tools.MsBuild.Generation.FrameworkDependent
{
    public class GenerateFeatureFileCodeBehind
    {
        public IEnumerable<string> GenerateFilesForProject(List<string> generatorPlugins, string projectPath, string projectFolder, string outputPath, string rootNamespace, List<string> featureFiles)
        {
            var generator = new FeatureFileCodeBehindGenerator();
            return generator.GenerateFilesForProject(projectPath, rootNamespace, featureFiles, generatorPlugins, projectFolder, outputPath);
        }
    }
}