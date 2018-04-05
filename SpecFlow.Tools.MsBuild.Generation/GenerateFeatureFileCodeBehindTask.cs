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


                    using (var client = new Client<IFeatureCodeBehindGenerator>(freePort))
                    {
                        await client.WaitForServer();


                        await client.Execute(c => c.InitializeProject(ProjectPath));


                        foreach (var featureFile in FeatureFiles)
                        {
                            string featureFileItemSpec = featureFile.ItemSpec;
                            var featureFileCodeBehindContent = await client.Execute(c => c.GenerateCodeBehindFile(featureFileItemSpec));

                            var resultedFile = WriteCodeBehindFile(OutputPath, featureFile, featureFileCodeBehindContent);

                            generatedFiles.Add(new TaskItem() {ItemSpec = FileSystemHelper.GetRelativePath(resultedFile, ProjectFolder)});
                        }


                        await client.ShutdownServer();
                    }
                }

                GeneratedFiles = generatedFiles.ToArray();

                return true;
            }
            catch (Exception e)
            {
                Log.LogWithNameTag(Log.LogError, e.Demystify().ToString());
                return false;
            }
        }


        private string WriteCodeBehindFile(string outputPath, ITaskItem featureFile, GeneratedCodeBehindFile generatedCodeBehindFile) //todo needs unit tests
        {
            Log.LogMessage(ProjectFolder);
            Log.LogMessage(outputPath);

            var path = Path.GetDirectoryName(Path.Combine(ProjectFolder, outputPath, featureFile.ItemSpec));

            if (string.IsNullOrEmpty(generatedCodeBehindFile.Filename))
            {
                Log.LogWithNameTag(Log.LogError, $"{featureFile.ItemSpec}has no generated filename");
                return null;
            }

            Log.LogWithNameTag(Log.LogMessage, path);

            var codeBehindFileLocation = Path.Combine(path, generatedCodeBehindFile.Filename);

            Log.LogWithNameTag(
                Log.LogMessage,
                $"Writing data to {codeBehindFileLocation}; path = {path}; generatedFilename = {generatedCodeBehindFile.Filename}");

            if (File.Exists(codeBehindFileLocation))
            {
                if (!FileSystemHelper.FileCompareContent(codeBehindFileLocation, generatedCodeBehindFile.Content))
                {
                    File.WriteAllText(codeBehindFileLocation, generatedCodeBehindFile.Content);
                }
            }
            else
            {
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                File.WriteAllText(codeBehindFileLocation, generatedCodeBehindFile.Content);
            }


            return codeBehindFileLocation;
        }
    }
}