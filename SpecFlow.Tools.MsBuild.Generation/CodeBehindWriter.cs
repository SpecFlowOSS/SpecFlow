using System;
using System.IO;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.CodeBehindGenerator;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class CodeBehindWriter
    {
        public CodeBehindWriter(TaskLoggingHelper log)
        {
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public TaskLoggingHelper Log { get; }

        public string WriteCodeBehindFile(IWriter outputWriter, ITaskItem featureFile, GeneratedCodeBehindFile generatedCodeBehindFile) //todo needs unit tests
        {
            Log.LogMessage(ProjectFolder);
            Log.LogMessage(outputPath);

            if (string.IsNullOrEmpty(generatedCodeBehindFile.Filename))
            {
                Log.LogWithNameTag(Log.LogError, $"{featureFile.ItemSpec}has no generated filename");
                return null;
            }

            Log.LogWithNameTag(Log.LogMessage, path);

            return outputWriter.WriteIfNotEqual(generatedCodeBehindFile.Content);
        }
    }
}
