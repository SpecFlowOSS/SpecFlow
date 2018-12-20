using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Resources;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Task
    {
        private readonly Func<IGenerateFeatureFileCodeBehind> _createGenerateFeatureFileCodeBehind;

        public GenerateFeatureFileCodeBehindTask()
        {
        }

        public GenerateFeatureFileCodeBehindTask(Func<IGenerateFeatureFileCodeBehind> createGenerateFeatureFileCodeBehind)
        {
            _createGenerateFeatureFileCodeBehind = createGenerateFeatureFileCodeBehind;
        }

        public GenerateFeatureFileCodeBehindTask(ResourceManager taskResources) : base(taskResources)
        {
        }

        public GenerateFeatureFileCodeBehindTask(ResourceManager taskResources, string helpKeywordPrefix) : base(taskResources, helpKeywordPrefix)
        {
        }

        [Required]
        public string ProjectPath { get; set; }

        [Required]
        public string RootNamespace { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        public ITaskItem[] GeneratorPlugins { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }

        public override bool Execute()
        {
            try
            {
                try
                {
                    var currentProcess = Process.GetCurrentProcess();

                    Log.LogWithNameTag(Log.LogMessage, $"process: {currentProcess.ProcessName}, pid: {currentProcess.Id}, CD: {Environment.CurrentDirectory}");

                    foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                    {
                        Log.LogWithNameTag(Log.LogMessage, "  " + assembly.FullName);
                    }
                }
                catch (Exception e)
                {
                    Log.LogWithNameTag(Log.LogMessage, $"Error when dumping process info: {e}");
                }


                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

                var generateFeatureFileCodeBehind = new GenerateFeatureFileCodeBehind(Log);

                Log.LogWithNameTag(Log.LogMessage, "Starting GenerateFeatureFileCodeBehind");

                var generatedFiles = new List<ITaskItem>();
                var generatorPlugins = GeneratorPlugins?.Select(gp => gp.ItemSpec).ToList() ?? new List<string>();

                var featureFiles = FeatureFiles?.Select(i => i.ItemSpec).ToList() ?? new List<string>();
                foreach (string s in generateFeatureFileCodeBehind.GenerateFilesForProject(generatorPlugins, ProjectPath, ProjectFolder, OutputPath, RootNamespace, featureFiles))
                {
                    generatedFiles.Add(new TaskItem() {ItemSpec = s});
                }

                GeneratedFiles = generatedFiles.ToArray();

                return Log.HasLoggedErrors;
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    if (e.InnerException is FileLoadException fle)
                    {
                        Log?.LogWithNameTag(Log.LogError, $"FileLoadException Filename: {fle.FileName}");
                        Log?.LogWithNameTag(Log.LogError, $"FileLoadException FusionLog: {fle.FusionLog}");
                        Log?.LogWithNameTag(Log.LogError, $"FileLoadException Message: {fle.Message}");
                    }

                    Log?.LogWithNameTag(Log.LogError, e.InnerException.ToString());
                }

                Log?.LogWithNameTag(Log.LogError, e.ToString());
                return false;
            }
            finally
            {
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
            }
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.LogWithNameTag(Log.LogMessage, args.Name);
            

            return null;
        }

    }


}