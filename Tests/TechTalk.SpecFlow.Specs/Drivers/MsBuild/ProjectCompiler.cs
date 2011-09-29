using System;
using System.Diagnostics;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Framework;

namespace TechTalk.SpecFlow.Specs.Drivers.MsBuild
{
    public class ProjectCompiler
    {
        public void Compile(Project project)
        {
            CompileOutProc(project);
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

        private void CompileOutProc(Project project)
        {
            string msBuildPath = Environment.ExpandEnvironmentVariables(string.Format(@"%WinDir%\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"));
            Console.WriteLine("Invoke MsBuild from {0}", msBuildPath);
            ProcessStartInfo psi = new ProcessStartInfo(msBuildPath, string.Format("/nologo /v:m \"{0}\"",
                                                                                   project.FullPath));
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardError = true;
            psi.UseShellExecute = false;
            psi.CreateNoWindow = true;
            var p = Process.Start(psi);

            Console.WriteLine(p.StandardOutput.ReadToEnd());

            p.WaitForExit();

            if (p.ExitCode > 0)
                throw new Exception("Build failed");
        }
    }
}