using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using TechTalk.SpecFlow.CodeBehindGenerator;
using TechTalk.SpecFlow.Rpc.Client;
using TechTalk.SpecFlow.Utils;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public class GenerateFeatureFileCodeBehindTask : Microsoft.Build.Utilities.Task
    {
        private static TimeSpan _timeout = TimeSpan.FromMinutes(10);
        private static int _timeOutInMilliseconds = Convert.ToInt32(_timeout.TotalMilliseconds);


        private const string GeneratorExe = "TechTalk.SpecFlow.CodeBehindGenerator.exe";
        private Process _externalProcess;
        private int _port = 3483;
        private StringBuilder _output;
        private AutoResetEvent _outputWaitHandle;
        private AutoResetEvent _errorWaitHandle;


        [Required]
        public string ProjectPath { get; set; }

        public string ProjectFolder => Path.GetDirectoryName(ProjectPath);
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
            var generatedFiles = new List<ITaskItem>();

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

                        var resultedFile = WriteCodeBehindFile(OutputPath, featureFile, featureFileCodeBehindContent);

                        generatedFiles.Add(new TaskItem(){ItemSpec = FileSystemHelper.GetRelativePath(resultedFile, ProjectFolder)});
                    }


                    await client.ShutdownServer();
                }

                StopOutOfProcGenerator();

                GeneratedFiles = generatedFiles.ToArray();

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
            if (_externalProcess.WaitForExit(_timeOutInMilliseconds) &&
                _outputWaitHandle.WaitOne(_timeOutInMilliseconds) &&
                _errorWaitHandle.WaitOne(_timeOutInMilliseconds))
            {
                Log.LogMessage("[SpecFlow]" + Environment.NewLine + _output);
            }
            else
            {
                Log.LogError($"[SpecFlow ]Process took longer than {_timeout.TotalMinutes} min to complete");
            }



            _outputWaitHandle.Dispose();
            _outputWaitHandle = null;
            _errorWaitHandle.Dispose();
            _errorWaitHandle = null;

            _output = null;

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

        private string WriteCodeBehindFile(string outputPath, ITaskItem featureFile, GeneratedCodeBehindFile generatedCodeBehindFile)
        {
            
            Log.LogMessage(ProjectFolder);
            Log.LogMessage(outputPath);

            var path = Path.GetDirectoryName(Path.Combine(ProjectFolder, outputPath, featureFile.ItemSpec));

            if (String.IsNullOrEmpty(generatedCodeBehindFile.Filename))
            {
                Log.LogError($"[SpecFlow] {featureFile.ItemSpec} has no generated filename");
                return null;
            }

            Log.LogMessage(path);

            var codeBehindFileLocation = Path.Combine(path, generatedCodeBehindFile.Filename); //todo subfolders of files

            Log.LogMessage($"[SpecFlow] Writing data to {codeBehindFileLocation}; path = {path}; generatedFilename = {generatedCodeBehindFile.Filename}");

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


        private void StartOutOfProcGenerator()
        {
            _output = new StringBuilder();
            _outputWaitHandle = new AutoResetEvent(false);
            _errorWaitHandle = new AutoResetEvent(false);



            string workingDirectory = Path.GetDirectoryName(GetType().Assembly.Location);

            var exe = Path.Combine(workingDirectory, GeneratorExe);

            Log.LogMessage($"Starting OutOfProcess generation process {exe} in {workingDirectory}");
            var processStartInfo = new ProcessStartInfo(exe, $"--port {_port}")
            {
                WorkingDirectory = workingDirectory,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = false,
            };

            _externalProcess = new Process
            {
                StartInfo = processStartInfo,
                EnableRaisingEvents = true,

            };
            _externalProcess.Exited += Process_Exited;


            _externalProcess.OutputDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    _outputWaitHandle.Set();
                }
                else
                {
                    _output.AppendLine(e.Data);
                }
            };
            _externalProcess.ErrorDataReceived += (sender, e) =>
            {
                if (e.Data == null)
                {
                    _errorWaitHandle.Set();
                }
                else
                {
                    _output.AppendLine(e.Data);
                }
            };


            try
            {
                _externalProcess.Start();
            }
            catch (Exception)
            {
                Log.LogError($"Error starting {_externalProcess.StartInfo.FileName} {_externalProcess.StartInfo.Arguments} in {_externalProcess.StartInfo.WorkingDirectory}");
                throw;
            }
            _externalProcess.BeginOutputReadLine();
            _externalProcess.BeginErrorReadLine();

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