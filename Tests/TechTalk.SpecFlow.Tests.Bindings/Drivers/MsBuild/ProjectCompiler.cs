using System;
using System.IO;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;
using Xunit.Abstractions;

namespace TechTalk.SpecFlow.Tests.Bindings.Drivers.MsBuild
{
    public class ProjectCompiler
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly VisualStudioFinder _visualStudioFinder;
        private readonly Folders _folders;

        public ProjectCompiler(ITestOutputHelper testOutputHelper, VisualStudioFinder visualStudioFinder, Folders folders)
        {
            _testOutputHelper = testOutputHelper;
            _visualStudioFinder = visualStudioFinder;
            _folders = folders;
        }

        public string LastCompilationOutput { get; private set; }

        public void Compile(string projectFile, string target = null)
        {
            CompileOutProc(projectFile, target);
        }

      

        private void CompileOutProc(string projectFile, string target = null)
        {
            RestoreNugetPackage(projectFile);

            var msBuildPath = _visualStudioFinder.FindMSBuild();
            _testOutputHelper.WriteLine("Invoke MsBuild from {0}", msBuildPath);

            var processHelper = new ProcessHelper();
            var targetArg = target == null ? "" : " /target:" + target;
            var exitCode = processHelper.RunProcess(msBuildPath, "/nologo /v:m \"{0}\" {1} /p:Configuration=Debug /p:Platform=AnyCpu", projectFile, targetArg);
            LastCompilationOutput = processHelper.ConsoleOutput;
            if (exitCode > 0)
            {
                _testOutputHelper.WriteLine(LastCompilationOutput);
                throw new Exception("Build failed");
            }
        }

        private void RestoreNugetPackage(string projectPath)
        {
            var processPath = Path.Combine(_folders.PackageFolder, "NuGet.CommandLine","4.3.0", "tools", "NuGet.exe");
            var commandLineArgs = $"restore {projectPath}";


            var nugetRestore = new ProcessHelper();
            nugetRestore.RunProcess(processPath, commandLineArgs);
        }

        private class ConsoleMsBuildLogger : ILogger
        {
            private readonly ITestOutputHelper _testOutputHelper;

            public ConsoleMsBuildLogger(ITestOutputHelper testOutputHelper)
            {
                _testOutputHelper = testOutputHelper;
            }

            public void Initialize(IEventSource eventSource)
            {
                eventSource.AnyEventRaised += (o, args) =>
                {
                    if (args.Message.StartsWith("SpecFlow"))
                        _testOutputHelper.WriteLine("MSBUILD: {0}", args.Message);
                };
                eventSource.ErrorRaised += (sender, args) => _testOutputHelper.WriteLine("MSBUILD: error {0}", args.Message);
            }

            public void Shutdown()
            {
            }

            public LoggerVerbosity Verbosity { get; set; }

            public string Parameters { get; set; }
        }
    }
}