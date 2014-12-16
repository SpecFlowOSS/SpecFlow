using System;
using System.Windows;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class VS2012RunnerGateway : ITestRunnerGateway
    {
        private readonly IProjectScopeFactory projectScopeFactory;
        private readonly VS2012RunnerGateway_CodeBehind runnerGateway_CodeBehind;
        private readonly VS2012RunnerGateway_TestWindowInvoke runnerGateway_TestWindowInvoke;

        public VS2012RunnerGateway(IProjectScopeFactory projectScopeFactory, DTE dte, IIdeTracer tracer, IServiceProvider serviceProvider)
        {
            this.projectScopeFactory = projectScopeFactory;
            runnerGateway_CodeBehind = new VS2012RunnerGateway_CodeBehind(dte, tracer);
            runnerGateway_TestWindowInvoke = new VS2012RunnerGateway_TestWindowInvoke(dte, tracer, serviceProvider);
        }

        private bool IsAllowDebugGeneratedFilesSet(ProjectItem projectItem)
        {
            var project = projectItem.ContainingProject;
            return project != null && IsAllowDebugGeneratedFilesSet(project);
        }

        private bool IsAllowDebugGeneratedFilesSet(Project project)
        {
            var projectScope = projectScopeFactory.GetProjectScope(project);
            return projectScope.SpecFlowProjectConfiguration.GeneratorConfiguration.AllowDebugGeneratedFiles;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope,
                                bool debug)
        {
            if (IsAllowDebugGeneratedFilesSet(projectItem))
                return runnerGateway_CodeBehind.RunScenario(projectItem, currentScenario, fileScope, debug);

            return runnerGateway_TestWindowInvoke.RunScenario(projectItem, currentScenario, fileScope, debug);
        }

        public bool RunFeatures(ProjectItem projectItem, bool debug)
        {
            if (IsAllowDebugGeneratedFilesSet(projectItem))
                return runnerGateway_CodeBehind.RunFeatures(projectItem, debug);

            return runnerGateway_TestWindowInvoke.RunFeatures(projectItem, debug);
        }

        public bool RunFeatures(Project project, bool debug)
        {
            if (IsAllowDebugGeneratedFilesSet(project))
                return runnerGateway_CodeBehind.RunFeatures(project, debug);

            return runnerGateway_TestWindowInvoke.RunFeatures(project, debug);
        }

        private class VS2012RunnerGateway_CodeBehind : CommandBasedTestRunnerGateway
        {
            protected override string GetRunInCurrentContextCommand(bool debug)
            {
                if (debug)
                    return "TestExplorer.DebugAllTestsInContext";

                return "TestExplorer.RunAllTestsInContext";
            }

            public VS2012RunnerGateway_CodeBehind(DTE dte, IIdeTracer tracer)
                : base(dte, tracer, true)
            {
            }
        }

        private class VS2012RunnerGateway_TestWindowInvoke : ITestRunnerGateway
        {
            private readonly DTE dte;
            private readonly IIdeTracer tracer;
            private readonly IServiceProvider serviceProvider;

            public VS2012RunnerGateway_TestWindowInvoke(DTE dte, IIdeTracer tracer, IServiceProvider serviceProvider)
            {
                this.dte = dte;
                this.tracer = tracer;
                this.serviceProvider = serviceProvider;
            }

            public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope,
                                    bool debug)
            {
                return RunInCurrentContext(VsxHelper.GetFileName(projectItem), debug, currentScenario.KeywordLine);
            }

            public bool RunFeatures(ProjectItem projectItem, bool debug)
            {
                var fileName = VsxHelper.GetFileName(projectItem);
                if (fileName != null && fileName.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase))
                    return RunInCurrentContext(fileName, debug);

                return RunCommand(debug ? "TestExplorer.DebugAllTestsInContext" : "TestExplorer.RunAllTestsInContext");
            }

            private bool RunInCurrentContext(string fileName, bool debug, int lineNumber = -1)
            {
                try
                {
                    const string testWindowAssemblyName =
                        "Microsoft.VisualStudio.TestWindow, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
                    const string testWindowInterfacesAssemblyName =
                        "Microsoft.VisualStudio.TestWindow.Interfaces, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
                    var tIRunCommandsExecutor = 
                        Type.GetType("Microsoft.VisualStudio.TestWindow.Controller.IRunCommandsExecutor, " + testWindowAssemblyName, true);

                    Type tHostContext = 
                        Type.GetType("Microsoft.VisualStudio.TestWindow.Extensibility.Model.HostContext, " + testWindowInterfacesAssemblyName, false) 
                        ?? 
                        Type.GetType("Microsoft.VisualStudio.TestWindow.Host.HostContext, " + testWindowAssemblyName, true); // support for VS2012 Update 1

                    var runCommandsExecutor = VsxHelper.ResolveMefDependency(serviceProvider, tIRunCommandsExecutor);
                    var hostContext = Activator.CreateInstance(tHostContext, fileName, lineNumber, -1);

                    string methodNameToInvoke = debug ? "OnInvokeDebugTestsInContext" : "OnInvokeRunTestsInContext";

                    var method = tIRunCommandsExecutor.GetMethod(methodNameToInvoke);
                    method.Invoke(runCommandsExecutor, new[] {hostContext});
                    return true;
                }
                catch (Exception ex)
                {
                    tracer.Trace("test tool error: {0}", this, ex);
                    return false;
                }
            }

            public bool RunFeatures(Project project, bool debug)
            {
                return RunCommand("TestExplorer.RunAllTests");
            }

            private bool RunCommand(string command)
            {
                try
                {
                    dte.ExecuteCommand(command);
                    return true;
                }
                catch (Exception ex)
                {
                    tracer.Trace("test tool error: {0}", this, ex);
                    return false;
                }
            }
        }
    }
}