using BoDi;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;

namespace TechTalk.SpecFlow.VsIntegration.Common.TestRunner
{
    public abstract class AutoTestRunnerGatewayLoader
    {
        private TestRunnerTool tool;

        public AutoTestRunnerGatewayLoader(TestRunnerTool tool)
        {
            this.tool = tool;
        }

        public abstract bool CanUse(Project project);

        public ITestRunnerGateway CreateTestRunner(IObjectContainer container)
        {
            return container.Resolve<ITestRunnerGateway>(tool.ToString());
        }
    }
}
