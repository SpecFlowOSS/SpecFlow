using System;
using System.IO;
using Microsoft.Build.Framework;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class FilePathGenerator
    {
        public static string GenerateFilePath(string projectFolder, string relativeOutputPath, string featureFileName, string generatedCodeBehindFileName)
        {
            string featureFileFullPath = Path.Combine(projectFolder, relativeOutputPath, featureFileName);
            string featureFileDirPath = Path.GetDirectoryName(featureFileFullPath) ?? throw new InvalidOperationException();

            return Path.Combine(
                featureFileDirPath,
                generatedCodeBehindFileName);
        }
    }
}
