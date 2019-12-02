using System;
using System.Collections.Generic;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class FeatureFileCodeBehindGenerator : IFeatureFileCodeBehindGenerator
    {
        private readonly FilePathGenerator _filePathGenerator;
        private readonly FeatureCodeBehindGenerator _featureCodeBehindGenerator;

        public FeatureFileCodeBehindGenerator(TaskLoggingHelper log, FeatureCodeBehindGenerator featureCodeBehindGenerator)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
            _featureCodeBehindGenerator = featureCodeBehindGenerator;
            _filePathGenerator = new FilePathGenerator();
        }

        public TaskLoggingHelper Log { get; }

        public IEnumerable<string> GenerateFilesForProject(
            IReadOnlyCollection<string> featureFiles,
            string projectFolder,
            string outputPath)
        {
            var codeBehindWriter = new CodeBehindWriter(null);

            if (featureFiles == null)
            {
                yield break;
            }

            foreach (var featureFile in featureFiles)
            {
                string featureFileItemSpec = featureFile;
                var generatorResult = _featureCodeBehindGenerator.GenerateCodeBehindFile(featureFileItemSpec);

                if (!generatorResult.Success)
                {
                    foreach (var error in generatorResult.Errors)
                    {
                        Log.LogError(
                            null,
                            null,
                            null,
                            featureFile,
                            error.Line,
                            error.LinePosition,
                            0,
                            0,
                            error.Message);
                    }

                    continue;
                }

                string targetFilePath = _filePathGenerator.GenerateFilePath(
                    projectFolder,
                    outputPath,
                    featureFile,
                    generatorResult.Filename);

                string resultedFile = codeBehindWriter.WriteCodeBehindFile(targetFilePath, featureFile, generatorResult);

                yield return FileSystemHelper.GetRelativePath(resultedFile, projectFolder);
            }
        }
    }
}
