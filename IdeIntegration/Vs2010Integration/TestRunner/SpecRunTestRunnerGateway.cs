using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Tracing.OutputWindow;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class SpecRunTestRunnerGateway : ITestRunnerGateway
    {
        private readonly IOutputWindowService outputWindowService;
        private readonly IIdeTracer tracer;
        private readonly IProjectScopeFactory projectScopeFactory;

        public SpecRunTestRunnerGateway(IOutputWindowService outputWindowService, IIdeTracer tracer, IProjectScopeFactory projectScopeFactory)
        {
            this.outputWindowService = outputWindowService;
            this.projectScopeFactory = projectScopeFactory;
            this.tracer = tracer;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope)
        {
            if (fileScope.HeaderBlock == null)
                return false;

            //TODO: support scenario outline
            string path = string.Format("Feature:{0}/Scenario:{1}", Escape(fileScope.HeaderBlock.Title), Escape(currentScenario.Title));
            return RunTests(projectItem.ContainingProject, "testpath:" + path);
        }

        private string Escape(string title)
        {
            return HttpUtility.UrlEncode(title);
        }

        public bool RunFeatures(ProjectItem projectItem)
        {
            var projectScope = (VsProjectScope)projectScopeFactory.GetProjectScope(projectItem.ContainingProject);
            if (!projectScope.FeatureFilesTracker.IsInitialized)
            {
                tracer.Trace("Feature file tracker is not yet initialized.", GetType().Name);
                return false;
            }

            string filter = string.Join(" | ", CollectPaths(projectScope, projectItem));
            if (string.IsNullOrEmpty(filter))
                return false;
            return RunTests(projectItem.ContainingProject, filter);
        }

        private IEnumerable<string> CollectPaths(VsProjectScope projectScope, ProjectItem projectItem)
        {
            var testPath = GetPath(projectScope, projectItem);
            if (testPath != null)
            {
                yield return testPath;
            }
            else
            {
                foreach (var subItemPath in VsxHelper.GetAllSubProjectItem(projectItem).Select(pi => GetPath(projectScope, pi)).Where(p => p != null))
                {
                    yield return subItemPath;
                }
            }
        }

        private string GetPath(VsProjectScope projectScope, ProjectItem projectItem)
        {
            var projectRelativePath = VsxHelper.GetProjectRelativePath(projectItem);
            if (projectRelativePath != null && projectRelativePath.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase))
            {
                var featureFileInfo = projectScope.FeatureFilesTracker.Files.FirstOrDefault(ffi => ffi.ProjectRelativePath == projectRelativePath);
                if (featureFileInfo == null)
                {
                    tracer.Trace("Feature file info not found.", GetType().Name);
                }
                else if (featureFileInfo.ParsedFeature == null)
                {
                    tracer.Trace("Feature file is not yet analyzed.", GetType().Name);
                }
                else
                {
                    string path = string.Format("testpath:Feature:{0}", Escape(featureFileInfo.ParsedFeature.Title));
                    return path;
                }
            }
            return null;
        }

        public bool RunFeatures(Project project)
        {
            return RunTests(project, null);
        }

        private static readonly string[] probingPaths = new[]
                                                            {
                                                                @".",
                                                                @"tools",
                                                                @"..\tools",
                                                                @"..\..\tools",
#if DEBUG
                                                                @"..\..\..\TechTalk.SpecRun.ConsoleRunner\bin\Debug"
#endif
                                                            };

        public bool RunTests(Project project, string filter)
        {
            var specRunRef = VsxHelper.GetReference(project, "TechTalk.SpecRun");
            if (specRunRef == null)
                return false;

            if (specRunRef.Path == null)
                return false;

            string runtimeFolder = Path.GetDirectoryName(specRunRef.Path);
            if (runtimeFolder == null)
                return false;

            var consolePath = probingPaths.Select(probingPath => Path.GetFullPath(Path.Combine(runtimeFolder, probingPath)))
                .Select(GetConsolePath).FirstOrDefault(cp => cp != null);

            if (consolePath == null)
                return false;

            var args = BuildCommandArgs(new ConsoleOptions
                                            {
                                                BaseFolder = VsxHelper.GetProjectFolder(project) + @"\bin\Debug",
                                                TestAssembly = VsxHelper.GetProjectAssemblyName(project) + ".dll",
                                                Filter = filter
                                            });
            ExecuteTests(consolePath, args);
            return true;
        }

        private string GetConsolePath(string probingPath)
        {
            var consolePath = Path.Combine(probingPath, "SpecRun.exe");
            if (!File.Exists(consolePath))
                return null;
            return consolePath;
        }

        public class ConsoleOptions
        {
            public string ConfigFile { get; set; }
            public string TestAssembly { get; set; }
            public string LogFile { get; set; }
            public string ReportFileName { get; set; }
            public string BaseFolder { get; set; }
            public string Filter { get; set; }
        }

        public string BuildCommandArgs(ConsoleOptions consoleOptions)
        {
            StringBuilder commandArgsBuilder = new StringBuilder();
            if (consoleOptions.ConfigFile != null)
                commandArgsBuilder.AppendFormat("\"/configFile:{0}\" ", consoleOptions.ConfigFile);
            if (consoleOptions.TestAssembly != null)
                commandArgsBuilder.AppendFormat("\"/assembly:{0}\" ", consoleOptions.TestAssembly);
            if (consoleOptions.LogFile != null)
                commandArgsBuilder.AppendFormat("\"/logFile:{0}\" ", consoleOptions.LogFile);
            if (consoleOptions.BaseFolder != null)
                commandArgsBuilder.AppendFormat("\"/baseFolder:{0}\" ", consoleOptions.BaseFolder);
            if (consoleOptions.ReportFileName != null)
                commandArgsBuilder.AppendFormat("\"/reportFile:{0}\" ", consoleOptions.ReportFileName);
            if (consoleOptions.Filter != null)
                commandArgsBuilder.AppendFormat("\"/filter:{0}\" ", consoleOptions.Filter);

            commandArgsBuilder.Append("/progress ");

            return commandArgsBuilder.ToString();
        }

        public void ExecuteTests(string consolePath, string commandArgs)
        {
            string command = string.Format("{0} {1}", consolePath, commandArgs);
            tracer.Trace(command, GetType().Name);

            var pane = outputWindowService.TryGetPane(OutputWindowDefinitions.SpecRunOutputWindowName);
            var displayResult = pane != null;

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = consolePath;
            process.StartInfo.Arguments = commandArgs;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            if (displayResult)
            {
                pane.Clear();
                pane.WriteLine(command);
                process.OutputDataReceived += (sender, args) =>
                                                  {
                                                      if (args.Data != null)
                                                      {
                                                          pane.WriteLine(args.Data);
                                                      }
                                                  };
            }

            process.Start();

            if (displayResult)
            {
                process.BeginOutputReadLine();
            }

            process.WaitForExit();
        }
    }
}