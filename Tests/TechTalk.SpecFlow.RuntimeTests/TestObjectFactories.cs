using System;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using System.Globalization;
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
            return CreateDefaultGlobalContainer(configurationProvider, registerGlobalMocks, new ContainerBuilder());
        }

        internal static IObjectContainer CreateDefaultTestThreadContainer(IRuntimeConfigurationProvider configurationProvider = null, Action<IObjectContainer> registerGlobalMocks = null, Action<IObjectContainer> registerTestThreadMocks = null)
        {
            return CreateDefaultTestThreadContainer(configurationProvider, registerGlobalMocks, registerTestThreadMocks, new ContainerBuilder());
        }

        internal static IObjectContainer CreateDefaultFeatureContainer(StringConfigProvider configurationHolder, IDefaultDependencyProvider defaultDependencyProvider = null)
        {
            var instance = new ContainerBuilder(defaultDependencyProvider);
            var testThreadContainer = CreateDefaultTestThreadContainer(configurationHolder, null, null, instance);

            CultureInfo cultureInfo = new CultureInfo("en-US");
            return instance.CreateFeatureContainer(testThreadContainer, new FeatureInfo(cultureInfo, "test feature info", "", ProgrammingLanguage.CSharp));
        }

        private static IObjectContainer CreateDefaultGlobalContainer(IRuntimeConfigurationProvider configurationProvider, Action<IObjectContainer> registerGlobalMocks, ContainerBuilder instance)
        {
            var globalContainer = instance.CreateGlobalContainer(configurationProvider);
            registerGlobalMocks?.Invoke(globalContainer);
            return globalContainer;
        }

        private static IObjectContainer CreateDefaultTestThreadContainer(IRuntimeConfigurationProvider configurationProvider, Action<IObjectContainer> registerGlobalMocks, Action<IObjectContainer> registerTestThreadMocks, ContainerBuilder instance)
        {
            var globalContainer = CreateDefaultGlobalContainer(configurationProvider, registerGlobalMocks, instance);
            var testThreadContainer = instance.CreateTestThreadContainer(globalContainer);
            registerTestThreadMocks?.Invoke(testThreadContainer);
            return testThreadContainer;
        }
    }
}
