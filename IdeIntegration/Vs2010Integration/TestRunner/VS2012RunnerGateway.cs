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
        private readonly DTE dte;
        private readonly IIdeTracer tracer;
        private readonly IServiceProvider serviceProvider;

        public VS2012RunnerGateway(DTE dte, IIdeTracer tracer, IServiceProvider serviceProvider)
        {
            this.dte = dte;
            this.tracer = tracer;
            this.serviceProvider = serviceProvider;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, IGherkinFileScope fileScope, bool debug)
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
                const string testWindowAssemblyName = "Microsoft.VisualStudio.TestWindow, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
                const string testWindowInterfacesAssemblyName = "Microsoft.VisualStudio.TestWindow.Interfaces, Version=11.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";
                var tIRunCommandsExecutor = Type.GetType("Microsoft.VisualStudio.TestWindow.Controller.IRunCommandsExecutor, " + testWindowAssemblyName, true);
                var tHostContext = Type.GetType("Microsoft.VisualStudio.TestWindow.Extensibility.Model.HostContext, " + testWindowInterfacesAssemblyName, true);

                var runCommandsExecutor = VsxHelper.ResolveMefDependency(serviceProvider, tIRunCommandsExecutor);
                var hostContext = Activator.CreateInstance(tHostContext, fileName, lineNumber, -1);

                string methodNameToInvoke = debug ? "OnInvokeDebugTestsInContext" : "OnInvokeRunTestsInContext";

                var method = tIRunCommandsExecutor.GetMethod(methodNameToInvoke);
                method.Invoke(runCommandsExecutor, new[] { hostContext });
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