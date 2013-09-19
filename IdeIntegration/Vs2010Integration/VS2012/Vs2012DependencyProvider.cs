using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.Vs2010Integration.TestRunner;

namespace TechTalk.SpecFlow.Vs2010Integration.VS2012
{
    internal class Vs2012DependencyProvider : DefaultDependencyProvider
    {
        public override void RegisterDefaults(BoDi.IObjectContainer container)
        {
            base.RegisterDefaults(container);

            container.RegisterTypeAs<MsTestRunnerGateway, ITestRunnerGateway>(TestRunnerTool.VisualStudio2010MsTest.ToString());
            container.RegisterTypeAs<ReSharper5TestRunnerGateway, ITestRunnerGateway>(TestRunnerTool.ReSharper5.ToString());
            container.RegisterTypeAs<VS2012RunnerGateway, ITestRunnerGateway>(TestRunnerTool.VisualStudio2012.ToString());
        }
    }
}
