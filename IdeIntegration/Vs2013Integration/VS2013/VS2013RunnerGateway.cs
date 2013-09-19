using System;
using System.Windows;
using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.Vs2010Integration.LanguageService;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using Microsoft.VisualStudio.TestWindow.Controller;
using Microsoft.VisualStudio.TestWindow.Host;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class VS2013RunnerGateway : ITestRunnerGateway
    {
        private readonly DTE dte;
        private readonly IIdeTracer tracer;
        private readonly IServiceProvider serviceProvider;

        public VS2013RunnerGateway(DTE dte, IIdeTracer tracer, IServiceProvider serviceProvider)
        {
            this.dte = dte;
            this.tracer = tracer;
            this.serviceProvider = serviceProvider;
        }

        public bool RunScenario(ProjectItem projectItem, IScenarioBlock currentScenario, ScenarioOutlineExamplesRow examplesRow, IGherkinFileScope fileScope, bool debug)
        {
            var line = currentScenario.KeywordLine;
            if (examplesRow != null && examplesRow.BlockRelativeLine >= 0)
                line += examplesRow.BlockRelativeLine;
            return RunInCurrentContext(VsxHelper.GetFileName(projectItem), debug, line);
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
                var runCommandsExecutor = VsxHelper.ResolveMefDependency<IRunCommandsExecutor>(serviceProvider);
                var hostContext = new HostContext(fileName, lineNumber >= 0 ? lineNumber + 1 : lineNumber, -1);

                if (debug)
                {
                    runCommandsExecutor.OnInvokeDebugTestsInContext(hostContext);
                }
                else
                {
                    runCommandsExecutor.OnInvokeRunTestsInContext(hostContext);
                }
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