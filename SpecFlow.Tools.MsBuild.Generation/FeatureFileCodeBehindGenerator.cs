﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class FeatureFileCodeBehindGenerator : IFeatureFileCodeBehindGenerator
    {
        private readonly FilePathGenerator _filePathGenerator;
        
        public FeatureFileCodeBehindGenerator(TaskLoggingHelper log)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
            _filePathGenerator = new FilePathGenerator();
        }

        public TaskLoggingHelper Log { get; }

        public IEnumerable<string> GenerateFilesForProject(
            string projectPath,
            string rootNamespace,
            List<string> featureFiles,
            List<string> generatorPlugins,
            string projectFolder,
            string outputPath)
        {
            using (var featureCodeBehindGenerator = new FeatureCodeBehindGenerator())
            {
                featureCodeBehindGenerator.InitializeProject(projectPath, rootNamespace, generatorPlugins);

                var codeBehindWriter = new CodeBehindWriter(null);

                if (featureFiles == null)
                {
                    yield break;
                }

                foreach (var featureFile in featureFiles)
                {
                    var featureFileItemSpec = featureFile;
                    var generatorResult = featureCodeBehindGenerator.GenerateCodeBehindFile(featureFileItemSpec);

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

                    var targetFilePath = _filePathGenerator.GenerateFilePath(
                        projectFolder,
                        outputPath,
                        featureFile,
                        generatorResult.Filename);

                    var resultedFile = codeBehindWriter.WriteCodeBehindFile(targetFilePath, featureFile, generatorResult);

                    yield return FileSystemHelper.GetRelativePath(resultedFile, projectFolder);
                }
            }

        }
    }
}
