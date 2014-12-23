using System;
using BoDi;
using TechTalk.SpecFlow.IdeIntegration.Options;
using TechTalk.SpecFlow.IdeIntegration.Tracing;

namespace TechTalk.SpecFlow.Vs2010Integration.TestRunner
{
    public interface ITestRunnerGatewayProvider
    {
        ITestRunnerGateway GetTestRunnerGateway(TestRunnerTool? runnerTool = null);
    }

    internal class TestRunnerGatewayProvider : ITestRunnerGatewayProvider
    {
        private readonly IObjectContainer container;
        private readonly IIntegrationOptionsProvider integrationOptionsProvider;
        private readonly IIdeTracer tracer;

        public TestRunnerGatewayProvider(IObjectContainer container, IIntegrationOptionsProvider integrationOptionsProvider, IIdeTracer tracer)
        {
            this.container = container;
            this.tracer = tracer;
            this.integrationOptionsProvider = integrationOptionsProvider;
        }

        public ITestRunnerGateway GetTestRunnerGateway(TestRunnerTool? runnerTool = null)
        {
            TestRunnerTool testRunnerTool = runnerTool ?? integrationOptionsProvider.GetOptions().TestRunnerTool;

            try
            {
                return container.Resolve<ITestRunnerGateway>(testRunnerTool.ToString());
            }
            catch (Exception ex)
            {
                tracer.Trace("Unable to resolve test runner gateway: " + ex, GetType().Name);
                return null;
            }
        }
    }
}