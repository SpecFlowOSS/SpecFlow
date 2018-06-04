using System;
using System.IO;
using Microsoft.Build.Framework;
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

        public string WriteCodeBehindFile(string outputPath, ITaskItem featureFile, GeneratedCodeBehindFile generatedCodeBehindFile) //todo needs unit tests
        {
            if (string.IsNullOrEmpty(generatedCodeBehindFile.Filename))
            {
                Log?.LogWithNameTag(Log.LogError, $"{featureFile.ItemSpec} has no generated filename");
                return null;
            }

            string directoryPath = Path.GetDirectoryName(outputPath) ?? throw new InvalidOperationException();
            Log.LogWithNameTag(Log.LogMessage, directoryPath);

            Log.LogWithNameTag(
                Log.LogMessage,
                $"Writing data to {outputPath}; path = {directoryPath}; generatedFilename = {generatedCodeBehindFile.Filename}");

            if (File.Exists(outputPath))
            {
                if (!FileSystemHelper.FileCompareContent(outputPath, generatedCodeBehindFile.Content))
                {
                    File.WriteAllText(outputPath, generatedCodeBehindFile.Content);
                }
            }
            else
            {
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }

                File.WriteAllText(outputPath, generatedCodeBehindFile.Content);
            }

            return outputPath;
        }
    }
}
