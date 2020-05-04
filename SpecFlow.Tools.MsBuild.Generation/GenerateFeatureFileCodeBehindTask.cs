using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.Analytics;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.Generator;

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
            var generateFeatureFileCodeBehindTaskContainerBuilder = new GenerateFeatureFileCodeBehindTaskContainerBuilder();
            var generatorPlugins = GeneratorPlugins?.Select(gp => gp.ItemSpec).Select(p => new GeneratorPluginInfo(p)).ToArray() ?? Array.Empty<GeneratorPluginInfo>();
            var featureFiles = FeatureFiles?.Select(i => i.ItemSpec).ToArray() ?? Array.Empty<string>();

            var msbuildInformationProvider = new MSBuildInformationProvider(MSBuildVersion);
            var generateFeatureFileCodeBehindTaskConfiguration = new GenerateFeatureFileCodeBehindTaskConfiguration(AnalyticsTransmitter, CodeBehindGenerator);
            var generateFeatureFileCodeBehindTaskInfo = new SpecFlowProjectInfo(generatorPlugins, featureFiles, ProjectPath, ProjectFolder, ProjectGuid, AssemblyName, OutputPath, RootNamespace, TargetFrameworks, TargetFramework);

            using (var taskRootContainer = generateFeatureFileCodeBehindTaskContainerBuilder.BuildRootContainer(Log, generateFeatureFileCodeBehindTaskInfo, msbuildInformationProvider, generateFeatureFileCodeBehindTaskConfiguration))
            {
                var assemblyResolveLoggerFactory = taskRootContainer.Resolve<IAssemblyResolveLoggerFactory>();
                using (assemblyResolveLoggerFactory.Build())
                {
                    var taskExecutor = taskRootContainer.Resolve<IGenerateFeatureFileCodeBehindTaskExecutor>();
                    var executeResult = taskExecutor.Execute();

                    if (!(executeResult is ISuccess<IReadOnlyCollection<ITaskItem>> success))
                    {
                        return false;
                    }

                    GeneratedFiles = success.Result.ToArray();
                    return true;
                }
            }
        }
    }
}
