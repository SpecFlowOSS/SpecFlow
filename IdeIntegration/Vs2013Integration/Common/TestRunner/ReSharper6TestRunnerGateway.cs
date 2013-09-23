using EnvDTE;
using System;
using System.Linq;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.VsIntegration.Common.TestRunner;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class ReSharper6GatewayLoader : AutoTestRunnerGatewayLoader
    {
        public ReSharper6GatewayLoader() : base (TestRunnerTool.ReSharper)
        {
        }

        public override bool CanUse(Project project)
        {
            var reSharperAssembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == "JetBrains.ReSharper.UnitTestFramework");
            return reSharperAssembly != null && reSharperAssembly.GetName().Version.Major <= 5;
        }
    }

    public class ReSharper6TestRunnerGateway : CommandBasedTestRunnerGateway
    {
        protected override string GetRunInCurrentContextCommand(bool debug)
        {
            if (debug)
                return "ReSharper.ReSharper_ReSharper_UnitTest_DebugContext";

            return "ReSharper.ReSharper_ReSharper_UnitTest_RunContext";
        }

        public ReSharper6TestRunnerGateway(DTE dte, IIdeTracer tracer)
            : base(dte, tracer)
        {
        }
    }
}