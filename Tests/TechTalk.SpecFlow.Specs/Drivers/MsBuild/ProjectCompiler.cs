using System;
using System.Diagnostics;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace TechTalk.SpecFlow.Specs.Drivers.MsBuild
{
    public class ProjectCompiler
    {
        public string LastCompilationOutput { get; private set; }

        public void Compile(Project project, string target = null)
        {
            CompileOutProc(project, target);
            //CompileInProc(project);
        }

        private class ConsoleMsBuildLogger : ILogger
        {
            public void Initialize(IEventSource eventSource)
            {
                eventSource.AnyEventRaised += (o, args) =>
                                                  {
                                                      if (args.Message.StartsWith("SpecFlow"))
                                                          Console.WriteLine("MSBUILD: {0}", args.Message);
                                                  };
                eventSource.ErrorRaised += (sender, args) => Console.WriteLine("MSBUILD: error {0}", args.Message);
            }

            public void Shutdown()
            {
            }

            public LoggerVerbosity Verbosity { get; set; }

            public string Parameters { get; set; }
        }

        private void CompileInProc(Project project)
        {
            if (!project.Build(new ConsoleMsBuildLogger()))
                throw new Exception("Build failed");
        }

        private void CompileOutProc(Project project, string target = null)
        {
            string msBuildPath = Environment.ExpandEnvironmentVariables(string.Format(@"%WinDir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"));
            Console.WriteLine("Invoke MsBuild from {0}", msBuildPath);

            ProcessHelper processHelper = new ProcessHelper();
            string targetArg = target == null ? "" : " /target:" + target;
            int exitCode = processHelper.RunProcess(msBuildPath, "/nologo /v:m \"{0}\" {1}", project.FullPath, targetArg);
            LastCompilationOutput = processHelper.ConsoleOutput;
            if (exitCode > 0)
                throw new Exception("Build failed");
        }
    }
}