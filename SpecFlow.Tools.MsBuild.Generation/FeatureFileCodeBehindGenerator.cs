using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    class FeatureFileCodeBehindGenerator
    {
        private readonly FilePathGenerator _filePathGenerator;

        public FeatureFileCodeBehindGenerator()
        {
            _filePathGenerator = new FilePathGenerator();
        }

        public IEnumerable<string> GenerateFilesForProject(string projectPath, string rootNamespace, List<string> featureFiles, List<string> generatorPlugins, string projectFolder, string outputPath)
        {

            using (var featureCodeBehindGenerator = new FeatureCodeBehindGenerator())
            {
                featureCodeBehindGenerator.InitializeProject(projectPath, rootNamespace, generatorPlugins);

                var codeBehindWriter = new CodeBehindWriter(null);
                if (featureFiles != null)
                {
                    foreach (var featureFile in featureFiles)
                    {
                        string featureFileItemSpec = featureFile;
                        var featureFileCodeBehind = featureCodeBehindGenerator.GenerateCodeBehindFile(featureFileItemSpec);

                        string targetFilePath = _filePathGenerator.GenerateFilePath(projectFolder, outputPath, featureFile, featureFileCodeBehind.Filename);
                        string resultedFile = codeBehindWriter.WriteCodeBehindFile(targetFilePath, featureFile, featureFileCodeBehind);

                        yield return FileSystemHelper.GetRelativePath(resultedFile, projectFolder);
                    }
                }
            }

        }
    }
}
