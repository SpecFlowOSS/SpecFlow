using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Windows.Forms;
using System.Windows.Threading;
using EnvDTE;
using EnvDTE80;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.Tracing.OutputWindow;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using Process = EnvDTE.Process;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Common.TestRunner;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class SpecRunGatewayLoader : AutoTestRunnerGatewayLoader
    {
        public SpecRunGatewayLoader()
            : base(TestRunnerTool.SpecRun)
        {
        }

        public override bool CanUse(Project project)
        {
            return VsxHelper.GetReference(project, "TechTalk.SpecRun") != null;
        }
    }

    public class SpecRunTestRunnerGateway : ITestRunnerGateway
    {
        private readonly IOutputWindowService outputWindowService;
        private readonly IIdeTracer tracer;
        private readonly IProjectScopeFactory projectScopeFactory;
        private readonly DTE2 dte;
        private readonly InstallServices installServices;

        public SpecRunTestRunnerGateway(IOutputWindowService outputWindowService, IIdeTracer tracer, IProjectScopeFactory projectScopeFactory, DTE2 dte, InstallServices installServices)
        {
            this.outputWindowService = outputWindowService;
            this.dte = dte;
            this.installServices = installServices;
            this.projectScopeFactory = projectScopeFactory;
            this.tracer = tracer;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, ScenarioOutlineExamplesRow examplesRow, IGherkinFileScope fileScope, bool debug)
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

            Version specRunVersion;
            if (!Version.TryParse(FileVersionInfo.GetVersionInfo(consolePath).FileVersion, out specRunVersion))
                specRunVersion = new Version(1, 1);

            var args = BuildCommandArgs(new ConsoleOptions
                                            {
                                                BaseFolder = VsxHelper.GetProjectFolder(project) + @"\bin\Debug", //TODO
                                                Target = VsxHelper.GetProjectAssemblyName(project) + ".dll",
                                                Filter = filter
                                            }, debug, specRunVersion);
            ExecuteTests(consolePath, args, debug, specRunVersion);
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

        static private readonly Version SpecRun12 = new Version(1, 2);

        public string BuildCommandArgs(ConsoleOptions consoleOptions, bool debug, Version specRunVersion)
        {
            var commandArgsBuilder = new StringBuilder("run ");
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

            string toolIntegration = specRunVersion >= SpecRun12 ? installServices.IdeIntegration.ToString().Replace("VisualStudio", "vs") : "vs2010";

            commandArgsBuilder.AppendFormat("/toolIntegration:{0} ", toolIntegration);
            if (debug)
                commandArgsBuilder.Append("/debug ");

            return commandArgsBuilder.ToString();
        }

        public class ExecutionContext
        {
            private readonly IOutputWindowPane pane;
            private readonly Dispatcher dispatcher;
            private readonly System.Diagnostics.Process process;
            private readonly bool debug;
            private readonly DTE2 dte;
            private readonly IIdeTracer tracer;
            private Version specRunVersion;
            private bool? shouldAttachToMain = true;

            public ExecutionContext(IOutputWindowPane pane, Dispatcher dispatcher, System.Diagnostics.Process process, bool debug, DTE2 dte, IIdeTracer tracer, Version specRunVersion)
            {
                this.pane = pane;
                this.dispatcher = dispatcher;
                this.process = process;
                this.debug = debug;
                this.dte = dte;
                this.tracer = tracer;
                this.specRunVersion = specRunVersion;

                if (specRunVersion >= SpecRun12)
                    shouldAttachToMain = null;

                process.OutputDataReceived += OnMessageReceived;
                process.ErrorDataReceived += OnMessageReceived;
            }

            private void OnMessageReceived(object sender, DataReceivedEventArgs args)
            {
                if (args.Data != null)
                {
                    string message = ProcessSpecRunMessage(args.Data);
                    if (!string.IsNullOrWhiteSpace(message) && pane != null)
                        dispatcher.BeginInvoke(new Action(() => pane.WriteLine(args.Data)), DispatcherPriority.ContextIdle);
                }
            }

            class SpecRunMessage
            {
                public readonly string Command;
                public readonly Dictionary<string, string> Args;

                public SpecRunMessage(Match match)
                {
                    Command = match.Groups["command"].Value.ToLowerInvariant();
                    Args = new Dictionary<string, string>();
                    var argsStrings = match.Groups["args"].Value.Trim().Split(new[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var argString in argsStrings)
                    {
                        var keyValue = argString.Split(new[] {'='}, 2);
                        if (keyValue.Length == 0 || string.IsNullOrWhiteSpace(keyValue[0]))
                            continue;
                        Args.Add(keyValue[0].Trim().ToLowerInvariant(), keyValue.Length == 1 || string.IsNullOrWhiteSpace(keyValue[1]) ? "" : keyValue[1]);
                    }
                }
            }

            static private readonly Regex specrunMessageRe = new Regex(@"##specrun\[(?<command>\w+)( (?<args>.*?))?(?<![^\|](\|\|)*\|)\]");

            private string ProcessSpecRunMessage(string message)
            {
                var matches = specrunMessageRe.Matches(message).Cast<Match>().ToArray();
                foreach (var srMessage in matches.Select(m => new SpecRunMessage(m)))
                {
                    if (srMessage.Command == "info")
                    {
                        if (srMessage.Args.ContainsKey("version"))
                        {
                            specRunVersion = new Version(srMessage.Args["version"]);
                            if (specRunVersion >= SpecRun12)
                                shouldAttachToMain = null;
                        }
                    }
                    if (srMessage.Command == "debug")
                    {
                        if (srMessage.Args.ContainsKey("skip"))
                        {
                            shouldAttachToMain = false;
                        }
                        else if (srMessage.Args.ContainsKey("ready"))
                        {
                            shouldAttachToMain = true;
                        }
                        else if (srMessage.Args.ContainsKey("pid"))
                        {
                            int pid = int.Parse(srMessage.Args["pid"], CultureInfo.InvariantCulture);
                            //attach to another process
                            AttachToProcess(pid);
                        }
                    }
                }

                foreach (var match in matches.OrderByDescending(m => m.Index))
                {
                    message = message.Remove(match.Index, match.Length);
                }

                return message;
            }

            public void Start()
            {
                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                if (debug)
                {
                    var maxWaitUntil = DateTime.Now.AddSeconds(30);
                    System.Threading.Thread.Sleep(100);
                    while (shouldAttachToMain == null && maxWaitUntil > DateTime.Now)
                    {
                        System.Threading.Thread.Sleep(100);
                    }
                    if (shouldAttachToMain != null && shouldAttachToMain.Value)
                        AttachToProcess(process.Id);
                }
            }

            private void AttachToProcess(int pid)
            {
                try
                {
                    var processes = dte.Debugger.LocalProcesses;
                    foreach (Process processToAttach in processes)
                        if (processToAttach.ProcessID == pid)
                        {
                            processToAttach.Attach();
                            return;
                        }
                    tracer.Trace("SpecRun process not found.", GetType().Name);
                }
                catch (Exception ex)
                {
                    tracer.Trace("Error attaching to SpecRun process. " + ex, GetType().Name);
                }
            }
        }

        public void ExecuteTests(string consolePath, string commandArgs, bool debug, Version specRunVersion)
        {
            string command = string.Format("{0} {1}", consolePath, commandArgs);
            tracer.Trace(command, GetType().Name);

            var pane = outputWindowService.TryGetPane(OutputWindowDefinitions.SpecRunOutputWindowName);
            var displayResult = pane != null;
            var dispatcher = Dispatcher.CurrentDispatcher;

            var process = new System.Diagnostics.Process
                {
                    StartInfo =
                        {
                            FileName = consolePath,
                            Arguments = commandArgs,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        }
                };

            var executionContext = new ExecutionContext(displayResult ? pane : null, dispatcher, process, debug, dte, tracer, specRunVersion);

            if (displayResult)
            {
                pane.Clear();
                pane.Activate();
                dte.ToolWindows.OutputWindow.Parent.Activate();
                pane.WriteLine(command);
            }

            executionContext.Start();

            // async execution: we do not call 'process.WaitForExit();'
        }

    }
}