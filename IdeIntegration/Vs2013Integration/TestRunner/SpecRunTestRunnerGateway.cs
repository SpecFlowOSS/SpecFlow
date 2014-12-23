using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Windows.Forms;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
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
        private readonly DTE2 dte;

        public SpecRunTestRunnerGateway(IOutputWindowService outputWindowService, IIdeTracer tracer, IProjectScopeFactory projectScopeFactory, DTE2 dte)
        {
            this.outputWindowService = outputWindowService;
            this.dte = dte;
            this.projectScopeFactory = projectScopeFactory;
            this.tracer = tracer;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope, bool debug)
        {
            if (fileScope.HeaderBlock == null)
                return false;

            //TODO: support scenario outline
            string path = string.Format("Feature:{0}/Scenario:{1}", Escape(fileScope.HeaderBlock.Title), Escape(currentScenario.Title));
            return RunTests(projectItem.ContainingProject, "testpath:" + path, debug);
        }

        private string Escape(string title)
        {
            return HttpUtility.UrlEncode(title);
        }

        public bool RunFeatures(ProjectItem projectItem, bool debug)
        {
            var projectScope = (VsProjectScope)projectScopeFactory.GetProjectScope(projectItem.ContainingProject);
            if (!projectScope.FeatureFilesTracker.IsInitialized)
            {
                MessageBox.Show("The feature files are not analyzed yet. Please wait.",
                                "SpecFlow",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                tracer.Trace("Feature file tracker is not yet initialized.", GetType().Name);
                return false;
            }

            string filter = string.Join(" | ", CollectPaths(projectScope, projectItem));
            if (string.IsNullOrEmpty(filter))
                return false;
            return RunTests(projectItem.ContainingProject, filter, debug);
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

        public bool RunFeatures(Project project, bool debug)
        {
            return RunTests(project, null, debug);
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

        private string FindConsolePath(Project project)
        {
            var specRunRef = VsxHelper.GetReference(project, "TechTalk.SpecRun");
            if (specRunRef == null)
                return null;

            if (specRunRef.Path == null)
                return null;

            string runtimeFolder = Path.GetDirectoryName(specRunRef.Path);
            if (runtimeFolder == null)
                return null;

            string fromRuntimePackage = probingPaths.Select(probingPath => Path.GetFullPath(Path.Combine(runtimeFolder, probingPath)))
                .Select(GetConsolePath).FirstOrDefault(cp => cp != null);
            if (fromRuntimePackage != null)
                return fromRuntimePackage;

            string nuGetPackagesFolder = Path.GetFullPath(Path.Combine(runtimeFolder, @"..\..\.."));
            if (!Directory.Exists(nuGetPackagesFolder))
                return null;
            var latestRunnerPackage = Directory.GetDirectories(nuGetPackagesFolder, "SpecRun.Runner.*").OrderByDescending(d => d)
                .Select(s => GetConsolePath(s + @"\tools")).FirstOrDefault(cp => cp != null);
            return latestRunnerPackage;
        }

        public bool RunTests(Project project, string filter, bool debug)
        {
            if (!VsxHelper.Build(project))
            {
                MessageBox.Show("Build failed.",
                                "SpecFlow",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return true;
            }

            var consolePath = FindConsolePath(project);
            if (consolePath == null)
            {
                MessageBox.Show("Unable to find SpecRun.exe.",
                                "SpecFlow",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Error);
                return true;
            }

            var args = BuildCommandArgs(new ConsoleOptions
                                            {
                                                BaseFolder = VsxHelper.GetProjectFolder(project) + @"\bin\Debug", //TODO
                                                Target = VsxHelper.GetProjectAssemblyName(project) + ".dll",
                                                Filter = filter
                                            }, debug);
            ExecuteTests(consolePath, args, debug);
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
            public string Target { get; set; }
            public string LogFile { get; set; }
            public string ReportFileName { get; set; }
            public string BaseFolder { get; set; }
            public string Filter { get; set; }
        }

        public string BuildCommandArgs(ConsoleOptions consoleOptions, bool debug)
        {
            StringBuilder commandArgsBuilder = new StringBuilder("run ");
            if (consoleOptions.Target == null)
                throw new InvalidOperationException();
            commandArgsBuilder.AppendFormat("\"{0}\" ", consoleOptions.Target);
            if (consoleOptions.LogFile != null)
                commandArgsBuilder.AppendFormat("\"/logFile:{0}\" ", consoleOptions.LogFile);
            if (consoleOptions.BaseFolder != null)
                commandArgsBuilder.AppendFormat("\"/baseFolder:{0}\" ", consoleOptions.BaseFolder);
            if (consoleOptions.ReportFileName != null)
                commandArgsBuilder.AppendFormat("\"/reportFile:{0}\" ", consoleOptions.ReportFileName);
            if (consoleOptions.Filter != null)
                commandArgsBuilder.AppendFormat("\"/filter:{0}\" ", consoleOptions.Filter);

            commandArgsBuilder.Append("/toolIntegration:vs2010 ");
            if (debug)
                commandArgsBuilder.Append("/debug ");

            return commandArgsBuilder.ToString();
        }

        public void ExecuteTests(string consolePath, string commandArgs, bool debug)
        {
            string command = string.Format("{0} {1}", consolePath, commandArgs);
            tracer.Trace(command, GetType().Name);

            var pane = outputWindowService.TryGetPane(OutputWindowDefinitions.SpecRunOutputWindowName);
            var displayResult = pane != null;
            var dispatcher = Dispatcher.CurrentDispatcher;

            var process = new System.Diagnostics.Process();
            process.StartInfo.FileName = consolePath;
            process.StartInfo.Arguments = commandArgs;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;

            if (displayResult)
            {
                pane.Clear();
                pane.Activate();
                dte.ToolWindows.OutputWindow.Parent.Activate();
                pane.WriteLine(command);
                process.OutputDataReceived += (sender, args) =>
                                                  {
                                                      if (args.Data != null)
                                                      {
                                                          dispatcher.BeginInvoke(new Action(() => pane.WriteLine(args.Data)), DispatcherPriority.ContextIdle);
                                                      }
                                                  };
            }

            process.Start();

            if (debug)
                AttachToProcess(process.Id);

            if (displayResult)
            {
                process.BeginOutputReadLine();
            }

            // async execution: we do not call 'process.WaitForExit();'
        }

        private void AttachToProcess(int pid)
        {
            try
            {
                var processes = dte.Debugger.LocalProcesses;
                foreach (Process process in processes)
                    if (process.ProcessID == pid)
                    {
                        process.Attach();
                        return;
                    }
                tracer.Trace("SpecRun process not found.", GetType().Name);
            }
            catch(Exception ex)
            {
                tracer.Trace("Error attaching to SpecRun process. " + ex, GetType().Name);
            }
        }
    }
}