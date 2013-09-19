using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;

namespace TechTalk.SpecFlow.VsIntegration.VS2013
{
    internal class Vs2013DependencyProvider : DefaultDependencyProvider
    {
        public override void RegisterDefaults(BoDi.IObjectContainer container)
        {
            base.RegisterDefaults(container);

            container.RegisterTypeAs<VS2013RunnerGateway, ITestRunnerGateway>(TestRunnerTool.VisualStudio2012.ToString());
        }
    }
}
