using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using TechTalk.SpecFlow.CodeBehindGenerator;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Microsoft.Build.Utilities.Task
    {
        private const string GeneratorExe = "TechTalk.SpecFlow.CodeBehindGenerator.exe";
        private Process _externalProcess;
        private int _port = 3483;


        [Required]
        public string ProjectPath { get; set; }
        public string OutputPath { get; set; }

        public ITaskItem[] FeatureFiles { get; set; }

        [Output]
        public ITaskItem[] GeneratedFiles { get; private set; }
        public override bool Execute()
        {
            Log.LogMessage("Starting GenerateFeatureFileCodeBehind");
            var asyncExecuteTask = AsyncExecute();

            
            return asyncExecuteTask.Result;
        }

        private async Task<bool> AsyncExecute()
        {
            try
            {
                StartOutOfProcGenerator();


                using (var client = new Client<IFeatureCodeBehindGenerator>(_port))
                {
                    while (true)
                    {
                        try
                        {
                            await client.Execute(c => c.Ping());
                            break;
                        }
                        catch (SocketException socketException)
                        {
                            Log.LogMessage($"[SpecFlow] retrying to contact code behind generator process - current result {socketException}");
                        }
                    }


                    await client.Execute(c => c.InitializeProject(ProjectPath));
                    

                    foreach (var featureFile in FeatureFiles)
                    {
                        string featureFileItemSpec = featureFile.ItemSpec;
                        var featureFileCodeBehindContent = await client.Execute(c => c.GenerateCodeBehindFile(featureFileItemSpec));

                        WriteCodeBehindFile(OutputPath, featureFile, featureFileCodeBehindContent);
                    }


                    await client.Execute(c => c.Shutdown());
                }

                StopOutOfProcGenerator();

                return true;
            }
            catch (Exception e)
            {
                Log.LogError(e.Demystify().ToString());
                return false;
            }
        }

        private void StopOutOfProcGenerator()
        {
            if (_externalProcess == null)
            {
                return;
            }

            if (_externalProcess.HasExited)
            {
                return;
            }

            _externalProcess.Kill();
        }

        private void WriteCodeBehindFile(string outputPath, ITaskItem featureFile, GeneratedCodeBehindFile generatedCodeBehindFile)
        {
            var path = Path.GetDirectoryName(featureFile.ItemSpec);
            var codeBehindFileLocation = Path.Combine(outputPath, generatedCodeBehindFile.Filename); //todo subfolders of files
            if (File.Exists(codeBehindFileLocation))
            {
                if (!FileSystemHelper.FileCompareContent(codeBehindFileLocation, generatedCodeBehindFile.Content))
                {
                    File.WriteAllText(codeBehindFileLocation, generatedCodeBehindFile.Content);
                }
            }
            else
            {
                File.WriteAllText(codeBehindFileLocation, generatedCodeBehindFile.Content);
            }
        }


        private void StartOutOfProcGenerator()
        {
            string workingDirectory = Path.GetDirectoryName(GetType().Assembly.Location);

            Log.LogMessage($"Starting OutOfProcess generation process {GeneratorExe} in {workingDirectory}");
            var processStartInfo = new ProcessStartInfo(GeneratorExe, $"--port {_port}")
            {
                CreateNoWindow = true,
                WorkingDirectory = workingDirectory
            };

            _externalProcess = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,
                
            };
            _externalProcess.Exited += Process_Exited;

            _externalProcess.Start();
            Log.LogMessage("OutOfProcess generation process started");

        }

        private void Process_Exited(object sender, EventArgs e)
        {
            Log.LogMessage($"OutOfProcess process has exited with exit code {_externalProcess.ExitCode}");

            _externalProcess.Exited -= Process_Exited;

            if (_externalProcess.ExitCode == 1) //port already used
            {
                _port++;
                //StartOutOfProcGenerator();
            }
        }
    }

}