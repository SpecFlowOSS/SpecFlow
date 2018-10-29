using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using SpecFlow.Tools.MsBuild.Generation.FrameworkDependent;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Task
    {
        
        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        public ITaskItem[] GeneratorPlugins { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; set; }

        public override bool Execute()
        {
            try
            {
                var generateFeatureFileCodeBehind = new GenerateFeatureFileCodeBehind();


                Log.LogWithNameTag(Log.LogMessage, "Starting GenerateFeatureFileCodeBehind");

                var generatedFiles = new List<ITaskItem>();
                var generatorPlugins = GeneratorPlugins?.Select(gp => gp.ItemSpec).ToList() ?? new List<string>();

                var featureFiles = FeatureFiles.Select(i => i.ItemSpec).ToList();
                foreach (string s in generateFeatureFileCodeBehind.GenerateFilesForProject(generatorPlugins, ProjectPath,ProjectFolder, OutputPath,RootNamespace, featureFiles))
                {
                    generatedFiles.Add(new TaskItem() { ItemSpec = s });
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