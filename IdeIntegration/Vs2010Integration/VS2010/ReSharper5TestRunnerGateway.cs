using EnvDTE;
using System;
using System.Linq;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Common.TestRunner;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class ReSharper5GatewayLoader : AutoTestRunnerGatewayLoader
    {
        public ReSharper5GatewayLoader()
            : base(TestRunnerTool.ReSharper5)
        {
        }

        public override bool CanUse(Project project)
        {
            var reSharperAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "JetBrains.ReSharper.UnitTestFramework");
            return reSharperAssembly != null;
        }
    }

    public class ReSharper5TestRunnerGateway : CommandBasedTestRunnerGateway
    {
        protected override string GetRunInCurrentContextCommand(bool debug)
        {
            if (debug)
                return "ReSharper.ReSharper_UnitTest_ContextDebug";

            return "ReSharper.ReSharper_UnitTest_ContextRun";
        }

        public ReSharper5TestRunnerGateway(DTE dte, IIdeTracer tracer)
            : base(dte, tracer)
        {
        }
    }
}