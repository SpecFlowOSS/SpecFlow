using System;
using EnvDTE;
using Microsoft.VisualStudio.TestWindow.Extensibility.Model;
using Microsoft.VisualStudio.TestWindow.Model;
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
            return RunInCurrentContext(VsxHelper.GetFileName(projectItem), debug);
        }

        public bool RunFeatures(ProjectItem projectItem, bool debug)
        {
            var fileName = VsxHelper.GetFileName(projectItem);
            if (fileName != null && fileName.EndsWith(".feature", StringComparison.InvariantCultureIgnoreCase))
                return RunInCurrentContext(fileName, debug);

            return RunCommand(debug ? "TestExplorer.DebugAllTestsInContext" : "TestExplorer.RunAllTestsInContext");
        }

        private bool RunInCurrentContext(string fileName, bool debug)
        {
            try
            {
                IRunCommandsExecutor runCommandsExecutor = VsxHelper.ResolveMefDependency<IRunCommandsExecutor>(serviceProvider);

                if (debug)
                    runCommandsExecutor.OnInvokeDebugTestsInContext(new HostContext(fileName));
                else
                    runCommandsExecutor.OnInvokeRunTestsInContext(new HostContext(fileName));
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