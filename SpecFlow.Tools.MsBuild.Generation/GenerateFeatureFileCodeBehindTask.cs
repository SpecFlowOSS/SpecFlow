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
        public GenerateFeatureFileCodeBehindTask()
        {
            CodeBehindGenerator = new FeatureFileCodeBehindGenerator(Log);
        }

        public IFeatureFileCodeBehindGenerator CodeBehindGenerator { get; set; }

        [Required]
        public string ProjectPath { get; set; }

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

                var generator = CodeBehindGenerator ?? new FeatureFileCodeBehindGenerator(Log);

                Log.LogWithNameTag(Log.LogMessage, "Starting GenerateFeatureFileCodeBehind");

                var generatorPlugins = GeneratorPlugins?.Select(gp => gp.ItemSpec).ToList() ?? new List<string>();

                var featureFiles = FeatureFiles?.Select(i => i.ItemSpec).ToList() ?? new List<string>();

                var generatedFiles = generator.GenerateFilesForProject(
                    ProjectPath,
                    RootNamespace,
                    featureFiles,
                    generatorPlugins,
                    ProjectFolder,
                    OutputPath);

                GeneratedFiles = generatedFiles.Select(file => new TaskItem { ItemSpec = file }).ToArray();

                return !Log.HasLoggedErrors;
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