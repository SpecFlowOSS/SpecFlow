using System.Reflection;
using BoDi;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow.MSTest.SpecFlowPlugin
{
    public class MsTestContainerBuilder : IContainerBuilder
    {
        private readonly IContainerBuilder _innerContainerBuilder;
        private readonly TestContext _testContext; 

        public MsTestContainerBuilder(TestContext testContext, IContainerBuilder innerContainerBuilder = null)
        {
            _testContext = testContext;
            _innerContainerBuilder = innerContainerBuilder ?? new ContainerBuilder();
        }

        public IObjectContainer CreateGlobalContainer(Assembly testAssembly, IRuntimeConfigurationProvider configurationProvider = null)
        {
            var container = _innerContainerBuilder.CreateGlobalContainer(testAssembly, configurationProvider);
            container.RegisterInstanceAs(_testContext);

            return container;
        }

        public IObjectContainer CreateTestThreadContainer(IObjectContainer globalContainer) => _innerContainerBuilder.CreateTestThreadContainer(globalContainer);

        public IObjectContainer CreateScenarioContainer(IObjectContainer testThreadContainer, ScenarioInfo scenarioInfo)
            => _innerContainerBuilder.CreateScenarioContainer(testThreadContainer, scenarioInfo);

        public IObjectContainer CreateFeatureContainer(IObjectContainer testThreadContainer, FeatureInfo featureInfo)
            => _innerContainerBuilder.CreateFeatureContainer(testThreadContainer, featureInfo);
    }
}
