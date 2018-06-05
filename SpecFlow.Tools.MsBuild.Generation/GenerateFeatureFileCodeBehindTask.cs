using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Microsoft.Build.Utilities.Task
    {
        private readonly int _startPort = 3483;

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }

        public override bool Execute()
        {
            Log.LogWithNameTag(Log.LogMessage, "Starting GenerateFeatureFileCodeBehind");
            
            return AsyncExecute();
        }

        private bool AsyncExecute()
        {
            var generatedFiles = new List<ITaskItem>();

            try
            {
                var filePathGenerator = new FilePathGenerator();

                

                using (var featureCodeBehindGenerator = new FeatureCodeBehindGenerator(Log))
                {
                    featureCodeBehindGenerator.InitializeProject(ProjectPath, RootNamespace);


                    var codeBehindWriter = new CodeBehindWriter(Log);
                    if (FeatureFiles != null)
                    {
                        foreach (var featureFile in FeatureFiles)
                        {
                            string featureFileItemSpec = featureFile.ItemSpec;
                            var featureFileCodeBehind = featureCodeBehindGenerator.GenerateCodeBehindFile(featureFileItemSpec);

                            string targetFilePath = filePathGenerator.GenerateFilePath(ProjectFolder, OutputPath, featureFile.ItemSpec, featureFileCodeBehind.Filename);
                            string resultedFile = codeBehindWriter.WriteCodeBehindFile(
                                targetFilePath,
                                featureFile,
                                featureFileCodeBehind);

                            generatedFiles.Add(new TaskItem { ItemSpec = FileSystemHelper.GetRelativePath(resultedFile, ProjectFolder) });
                        }
                    }
                }


                GeneratedFiles = generatedFiles.ToArray();

                return true;
            }
            catch (Exception e)
            {
                Log?.LogWithNameTag(Log.LogError, e.Demystify().ToString());
                return false;
            }
        }
    }
}