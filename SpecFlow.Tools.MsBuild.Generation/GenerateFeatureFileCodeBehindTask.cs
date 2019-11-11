using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using BoDi;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Project;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Task
    {
        public IFeatureFileCodeBehindGenerator CodeBehindGenerator { get; set; }
        public IAnalyticsTransmitter AnalyticsTransmitter { get; set; }

        [Required]
        public string ProjectPath { get; set; }

        public string RootNamespace { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        public ITaskItem[] GeneratorPlugins { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }
        
        public string MSBuildVersion { get; set; }
        public string AssemblyName { get; set; }
        public string TargetFrameworks { get; set; }
        public string TargetFramework { get; set; }
        public string ProjectGuid { get; set; }

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

                Log.LogWithNameTag(Log.LogMessage, "Starting GenerateFeatureFileCodeBehind");

                var generatorPlugins = GeneratorPlugins?.Select(gp => gp.ItemSpec).ToList() ?? new List<string>();

                var featureFiles = FeatureFiles?.Select(i => i.ItemSpec).ToList() ?? new List<string>();

                var specFlowProject = MsBuildProjectReader.LoadSpecFlowProjectFromMsBuild(Path.GetFullPath(ProjectPath), RootNamespace);
                using (var container = GeneratorContainerBuilder.CreateContainer(specFlowProject.ProjectSettings.ConfigurationHolder, specFlowProject.ProjectSettings, generatorPlugins))
                {
                    RegisterGenerationAndAnalyticsSpecific(container);

                    GeneratedFiles = GenerateCodeBehindFilesForProject(container, featureFiles);

                    TransmitProjectCompilingEvent(container);
                }

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

        private ITaskItem[] GenerateCodeBehindFilesForProject(IObjectContainer container, List<string> featureFiles)
        {
            var generator = container.Resolve<IFeatureFileCodeBehindGenerator>();
            var generatedFiles = generator.GenerateFilesForProject(
                featureFiles,
                ProjectFolder,
                OutputPath);

            return generatedFiles.Select(file => new TaskItem { ItemSpec = file }).ToArray();
        }

        private void TransmitProjectCompilingEvent(IObjectContainer container)
        {
            try
            {
                var analyticsTransmitter = container.Resolve<IAnalyticsTransmitter>();
                var eventProvider = container.Resolve<IAnalyticsEventProvider>();

                var projectCompilingEvent = eventProvider.CreateProjectCompilingEvent(MSBuildVersion, AssemblyName, TargetFrameworks, TargetFramework, ProjectGuid);
                analyticsTransmitter.TransmitSpecflowProjectCompilingEvent(projectCompilingEvent);
            }
            catch (Exception)
            {
                // catch all exceptions since we do not want to break the build simply because event creation failed
            }
        }

        private void RegisterGenerationAndAnalyticsSpecific(IObjectContainer container)
        {
            container.RegisterInstanceAs(Log);
            container.RegisterTypeAs<AnalyticsEventProvider, IAnalyticsEventProvider>();

            if (CodeBehindGenerator is null)
            {
                container.RegisterTypeAs<FeatureFileCodeBehindGenerator, IFeatureFileCodeBehindGenerator>();
            }
            else
            {
                container.RegisterInstanceAs(CodeBehindGenerator);
            }

            if (AnalyticsTransmitter is null)
            {
                container.RegisterTypeAs<AnalyticsTransmitter, IAnalyticsTransmitter>();
            }
            else
            {
                container.RegisterInstanceAs(AnalyticsTransmitter);
            }
        }

        private System.Reflection.Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            Log.LogWithNameTag(Log.LogMessage, args.Name);

            return null;
        }

    }
}