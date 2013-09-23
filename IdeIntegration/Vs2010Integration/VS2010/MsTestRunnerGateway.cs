using EnvDTE;
using TechTalk.SpecFlow.IdeIntegration.Tracing;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.VsIntegration.Common.TestRunner;
using TechTalk.SpecFlow.Vs2010Integration.Utils;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public class VisualStudio2010MsTestGatewayLoader : AutoTestRunnerGatewayLoader
    {
        public VisualStudio2010MsTestGatewayLoader()
            : base(TestRunnerTool.VisualStudio2010MsTest)
        {

        }

        public override bool CanUse(Project project)
        {
            return VsxHelper.GetReference(project, "Microsoft.VisualStudio.QualityTools.UnitTestFramework") != null;
        }
    }
    public class MsTestRunnerGateway : CommandBasedTestRunnerGateway
    {
        protected override string GetRunInCurrentContextCommand(bool debug)
        {
            if (debug)
                return "Test.DebugTestsInCurrentContext";

            return "Test.RunTestsInCurrentContext";
        }

        public MsTestRunnerGateway(DTE dte, IIdeTracer tracer) : base(dte, tracer)
        {
        }

        protected override int GetFeatureCodeBehindLine(TextDocument codeBehindDoc)
        {
            return GetCodeBehindLine(codeBehindDoc, 1);
        }
    }
}