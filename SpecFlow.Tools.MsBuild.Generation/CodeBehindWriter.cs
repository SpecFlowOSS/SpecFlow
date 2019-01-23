using System;
using System.IO;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class CodeBehindWriter
    {
        public CodeBehindWriter(TaskLoggingHelper log)
        {
            Log = log;
        }

        public TaskLoggingHelper Log { get; }

        public string WriteCodeBehindFile(string outputPath, string featureFile, TestFileGeneratorResult testFileGeneratorResult) //todo needs unit tests
        {
            if (string.IsNullOrEmpty(testFileGeneratorResult.Filename))
            {
                Log?.LogWithNameTag(Log.LogError, $"{featureFile} has no generated filename");
                return null;
            }

            string directoryPath = Path.GetDirectoryName(outputPath) ?? throw new InvalidOperationException();
            Log?.LogWithNameTag(Log.LogMessage, directoryPath);

            Log?.LogWithNameTag(Log.LogMessage, $"Writing data to {outputPath}; path = {directoryPath}; generatedFilename = {testFileGeneratorResult.Filename}");

            if (File.Exists(outputPath))
            {
                if (!FileSystemHelper.FileCompareContent(outputPath, testFileGeneratorResult.GeneratedTestCode))
                {
                    File.WriteAllText(outputPath, testFileGeneratorResult.GeneratedTestCode);
                }
            }
            else
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(outputPath, testFileGeneratorResult.GeneratedTestCode);
            }

            return outputPath;
        }
    }
}
