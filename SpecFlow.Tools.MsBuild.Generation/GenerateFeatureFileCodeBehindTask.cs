using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.CodeBehindGenerator;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Rpc.Shared;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Microsoft.Build.Utilities.Task
    {
        private readonly int _startPort = 3483;

        [Required]
        public string ProjectPath { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }

        public override bool Execute()
        {
            Log.LogWithNameTag(Log.LogMessage, "Starting GenerateFeatureFileCodeBehind");
            var asyncExecuteTask = AsyncExecute();
            return asyncExecuteTask.Result;
        }

        private async Task<bool> AsyncExecute()
        {
            var generatedFiles = new List<ITaskItem>();

            try
            {
                using (var outOfProcessServer = new OutOfProcessServer(Log))
                {
                    var freePort = FindFreePort.GetAvailablePort(_startPort);

                    outOfProcessServer.Start(freePort);
                    var filePathGenerator = new FilePathGenerator();


                    var usedPort = outOfProcessServer.WaitForPort();

                    using (var client = new Client<IFeatureCodeBehindGenerator>(usedPort))
                    {
                        await client.WaitForServer();
                        await client.Execute(c => c.InitializeProject(ProjectPath));

                        var codeBehindWriter = new CodeBehindWriter(Log);
                        foreach (var featureFile in FeatureFiles)
                        {
                            string featureFileItemSpec = featureFile.ItemSpec;
                            var featureFileCodeBehind = await client.Execute(c => c.GenerateCodeBehindFile(featureFileItemSpec));

                            string targetFilePath = filePathGenerator.GenerateFilePath(ProjectFolder, OutputPath, featureFile.ItemSpec, featureFileCodeBehind.Filename);
                            string resultedFile = codeBehindWriter.WriteCodeBehindFile(
                                targetFilePath,
                                featureFile,
                                featureFileCodeBehind);

                            generatedFiles.Add(new TaskItem { ItemSpec = FileSystemHelper.GetRelativePath(resultedFile, ProjectFolder) });
                        }


                        await client.ShutdownServer();
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