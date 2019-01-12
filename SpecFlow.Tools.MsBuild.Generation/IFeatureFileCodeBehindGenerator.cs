using System.Collections.Generic;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IFeatureFileCodeBehindGenerator
    {
        IEnumerable<string> GenerateFilesForProject(string projectPath, string rootNamespace, List<string> featureFiles, List<string> generatorPlugins, string projectFolder, string outputPath);
    }
}