using System;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.RuntimeTests.Infrastructure;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public static class TestObjectFactories
    {
        static internal TestRunner CreateTestRunner(out IObjectContainer createThreadContainer, Action<IObjectContainer> registerTestThreadMocks = null, Action<IObjectContainer> registerGlobalMocks = null)
        {
            createThreadContainer = CreateDefaultTestThreadContainer(registerTestThreadMocks: registerTestThreadMocks, registerGlobalMocks: registerGlobalMocks);
            return (TestRunner)createThreadContainer.Resolve<ITestRunner>();
        }

        static internal TestRunner CreateTestRunner(Action<IObjectContainer> registerTestThreadMocks = null, Action<IObjectContainer> registerGlobalMocks = null)
        {
            return CreateTestRunner(out _, registerTestThreadMocks, registerGlobalMocks);
        }

        internal static IObjectContainer CreateDefaultGlobalContainer(IRuntimeConfigurationProvider configurationProvider = null, Action<IObjectContainer> registerGlobalMocks = null)
        {
            var instance = new ContainerBuilder();
            var globalContainer = instance.CreateGlobalContainer(configurationProvider);
            registerGlobalMocks?.Invoke(globalContainer);
            return globalContainer;
        }

        internal static IObjectContainer CreateDefaultTestThreadContainer(IRuntimeConfigurationProvider configurationProvider = null, Action<IObjectContainer> registerGlobalMocks = null, Action<IObjectContainer> registerTestThreadMocks = null)
        {
            var instance = new ContainerBuilder();
            var globalContainer = CreateDefaultGlobalContainer(configurationProvider, registerGlobalMocks);
            var testThreadContainer = instance.CreateTestThreadContainer(globalContainer);
            registerTestThreadMocks?.Invoke(testThreadContainer);
            return testThreadContainer;
        }

        internal static IObjectContainer CreateDefaultScenarioContainer(StringConfigProvider configurationHolder)
        {
            var instance = new ContainerBuilder();
            var testThreadContainer = CreateDefaultTestThreadContainer(configurationHolder);

            return instance.CreateScenarioContainer(testThreadContainer, new ScenarioInfo("test scenario info", "test_scenario_description"));
        }
    }
}
