using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
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