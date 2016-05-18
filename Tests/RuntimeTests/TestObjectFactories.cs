using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.RuntimeTests.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public static class TestObjectFactories
    {
        static internal TestRunner CreateTestRunner(out IObjectContainer container, Action<IObjectContainer> registerMocks = null)
        {
            container = CreateDefaultTestThreadContainer();

            if (registerMocks != null)
                registerMocks(container);

            return (TestRunner)container.Resolve<ITestRunner>();
        }

        static internal TestRunner CreateTestRunner(Action<IObjectContainer> registerMocks = null)
        {
            IObjectContainer container;
            return CreateTestRunner(out container, registerMocks);
        }
        static internal TestRunner CreateTestRunnerRegisteringGlobalMocks(out IObjectContainer container, Action<IObjectContainer> registerMocks)
        {
            container = CreateDefaultTestThreadContainer(null,registerMocks);

            return (TestRunner)container.Resolve<ITestRunner>();
        }


        internal static IObjectContainer CreateDefaultGlobalContainer(IRuntimeConfigurationProvider configurationProvider = null, Action<IObjectContainer> registerGlobalMocks = null)
        {
            var instance = new ContainerBuilder();
            var globalContainer = instance.CreateGlobalContainer(configurationProvider);

            if(registerGlobalMocks!=null)
                registerGlobalMocks(globalContainer);

            return globalContainer;
        }

        internal static IObjectContainer CreateDefaultTestThreadContainer(IRuntimeConfigurationProvider configurationProvider = null, Action<IObjectContainer> registerGlobalMocks = null)
        {
            var instance = new ContainerBuilder();
            var globalContainer = CreateDefaultGlobalContainer(configurationProvider, registerGlobalMocks);
            return instance.CreateTestThreadContainer(globalContainer);
        }

        internal static IObjectContainer CreateDefaultScenarioContainer(StringConfigProvider configurationHolder)
        {
            var instance = new ContainerBuilder();
            var testThreadContainer = CreateDefaultTestThreadContainer(configurationHolder);

            return instance.CreateScenarioContainer(testThreadContainer, new ScenarioInfo("test scenario info"));
        }
    }
}
