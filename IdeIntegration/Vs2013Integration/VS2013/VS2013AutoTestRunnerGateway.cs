using BoDi;
using EnvDTE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.IdeIntegration.Install;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;
using TechTalk.SpecFlow.Vs2010Integration.Utils;
using TechTalk.SpecFlow.VsIntegration.Common.TestRunner;

namespace TechTalk.SpecFlow.VsIntegration.VS2013
{
    public class SpecRunWithVS2013GatewayLoader : AutoTestRunnerGatewayLoader
    {
        public SpecRunWithVS2013GatewayLoader()
            : base(TestRunnerTool.VisualStudio2012)
        {
        }

        public override bool CanUse(Project project)
        {
            return VsxHelper.GetReference(project, "TechTalk.SpecRun") != null; // would make sense to check for version 1.2 or above, but too complicated
        }
    }


    public class VS2013AutoTestRunnerGateway : AutoTestRunnerGateway
    {
        public VS2013AutoTestRunnerGateway(IObjectContainer container, InstallServices installServices) : base(container, installServices)
        {

        }

        protected override IEnumerable<Common.TestRunner.AutoTestRunnerGatewayLoader> GetLoaders()
        {
            yield return new SpecRunWithVS2013GatewayLoader();
            yield return new SpecRunGatewayLoader();
            yield return new ReSharper6GatewayLoader();
            yield return new VisualStudio2013GatewayLoader();
        }
    }
}
