using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class MsTestRunnerGateway : CommandBasedTestRunnerGateway
    {
        protected override string RunInCurrentContextCommand
        {
            get { return "Test.RunTestsInCurrentContext"; }
        }

        public MsTestRunnerGateway(DTE dte, IIdeTracer tracer) : base(dte, tracer)
        {
        }
    }
}