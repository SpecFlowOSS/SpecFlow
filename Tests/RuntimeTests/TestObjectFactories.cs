using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.RuntimeTests
{
    public static class TestObjectFactories
    {
        static internal TestRunner CreateTestRunner(out IObjectContainer container, Action<IObjectContainer> registerMocks = null)
        {
            container = CreateDefaultTestRunnerContainer();

            if (registerMocks != null)
                registerMocks(container);

            return (TestRunner)container.Resolve<ITestRunner>();
        }

        static internal TestRunner CreateTestRunner(Action<IObjectContainer> registerMocks = null)
        {
            IObjectContainer container;
            return CreateTestRunner(out container, registerMocks);
        }

        internal static IObjectContainer CreateDefaultGlobalContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var instance = new TestRunContainerBuilder();
            return instance.CreateContainer(configurationProvider);
        }

        internal static IObjectContainer CreateDefaultTestRunnerContainer(IRuntimeConfigurationProvider configurationProvider = null)
        {
            var instance = new TestRunContainerBuilder();
            var globalContainer = CreateDefaultGlobalContainer(configurationProvider);
            globalContainer.Resolve<TraceListenerQueue>(); //TODO[thread-safety]: remove when bodi fixed
            return instance.CreateTestRunnerContainer(globalContainer);
        }
    }
}
