using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class ReSharperTestRunnerGateway : CommandBasedTestRunnerGateway
    {
        protected override string RunInCurrentContextCommand
        {
            get { return "ReSharper.ReSharper_UnitTest_ContextRun"; }
        }

        public ReSharperTestRunnerGateway(DTE dte, IIdeTracer tracer)
            : base(dte, tracer)
        {
        }
    }
}