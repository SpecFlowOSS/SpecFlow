using System;
using BoDi;
using TechTalk.SpecFlow.IdeIntegration.Options;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public interface ITestRunnerGatewayProvider
    {
        ITestRunnerGateway GetTestRunnerGateway();
    }

    internal class TestRunnerGatewayProvider : ITestRunnerGatewayProvider
    {
        private readonly IObjectContainer container;
        private readonly IIntegrationOptionsProvider integrationOptionsProvider;

        public TestRunnerGatewayProvider(IObjectContainer container, IIntegrationOptionsProvider integrationOptionsProvider)
        {
            this.container = container;
            this.integrationOptionsProvider = integrationOptionsProvider;
        }

        public ITestRunnerGateway GetTestRunnerGateway()
        {
            TestRunnerTool testRunnerTool = integrationOptionsProvider.GetOptions().TestRunnerTool;
            return container.Resolve<ITestRunnerGateway>(testRunnerTool.ToString());
        }
    }
}