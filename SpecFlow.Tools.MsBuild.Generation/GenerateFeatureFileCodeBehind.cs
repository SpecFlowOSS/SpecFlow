using System;
using System.Collections.Generic;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public interface IGenerateFeatureFileCodeBehind
    {
        IEnumerable<string> GenerateFilesForProject(List<string> generatorPlugins, string projectPath, string projectFolder, string outputPath, string rootNamespace, List<string> featureFiles);
    }

    public class GenerateFeatureFileCodeBehind : IGenerateFeatureFileCodeBehind
    {
        private readonly TaskLoggingHelper _log;

        public GenerateFeatureFileCodeBehind(TaskLoggingHelper log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public IEnumerable<string> GenerateFilesForProject(List<string> generatorPlugins, string projectPath, string projectFolder, string outputPath, string rootNamespace, List<string> featureFiles)
        {
            var generator = new FeatureFileCodeBehindGenerator(_log);
            return generator.GenerateFilesForProject(projectPath, rootNamespace, featureFiles, generatorPlugins, projectFolder, outputPath);
        }
    }
}