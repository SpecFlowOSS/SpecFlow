using System;
using System.Collections.Generic;
using System.Reflection;
using TechTalk.SpecFlow.TestFrameworkIntegration;

namespace TechTalk.SpecFlow
{
    internal class ObjectContainer
    {
        #region Configuration
        private static Configuration configuration = null;

        public static Configuration Configuration
        {
            get
            {
                if (configuration == null)
                {
                    configuration = Configuration.LoadFromConfigFile();
                }
                return configuration;
            }
        }
        #endregion

        #region TestRunner
        private static ITestRunner testRunner = null;

        private static ITestRunner CreateTestRunner()
        {
            return new TestRunner(); //TODO: factory from config?
        }

        public static ITestRunner TestRunner
        {
            get 
            {
                return EnsureTestRunner(Assembly.GetCallingAssembly());
            }
            internal set
            {
                testRunner = value;
            }
        }

        internal static ITestRunner EnsureTestRunner(Assembly callingAssembly)
        {
            if (testRunner == null)
            {
                testRunner = CreateTestRunner();

                List<Assembly> bindingAssemblies = new List<Assembly>();
                bindingAssemblies.Add(callingAssembly); //TODO: add more assemblies from config
                testRunner.InitializeTestRunner(bindingAssemblies.ToArray());
            }
            return testRunner;
        }

        #endregion

        #region FeautreContext

        private static FeatureContext featureContext = null;

        static public FeatureContext FeatureContext
        {
            get
            {
                if (featureContext == null)
                    return null;
                return featureContext;
            }
            internal set
            {
                if (featureContext != null)
                {
                    if (value != null)
                        TestTracer.Warning("The previous feature context was not disposed.");
                    DisposeFeatureContext();
                }

                featureContext = value;
            }
        }

        private static void DisposeFeatureContext()
        {
            ((IDisposable)featureContext).Dispose();
            featureContext = null;
        }

        #endregion

        #region ScenarioContext

        private static ScenarioContext scenarioContext = null;

        static public ScenarioContext ScenarioContext
        {
            get
            {
                if (scenarioContext == null)
                    return null;
                return scenarioContext;
            }
            internal set
            {
                if (scenarioContext != null)
                {
                    if (value != null)
                        TestTracer.Warning("The previous scenario context was not disposed.");
                    DisposeScenarioContext();
                }

                scenarioContext = value;
            }
        }

        private static void DisposeScenarioContext()
        {
            ((IDisposable)scenarioContext).Dispose();
            scenarioContext = null;
        }

        #endregion

        #region TestTracer
        private static TestTracer testTracer = null;

        public static TestTracer TestTracer
        {
            get
            {
                if (testTracer == null)
                {
                    testTracer = new ConsoleTestTracer();
                }
                return testTracer;
            }
        }
        #endregion

        #region TestFrameworkIntegration
        private static ITestFrameworkIntegration testFrameworkIntegration = null;

        public static ITestFrameworkIntegration TestFrameworkIntegration
        {
            get
            {
                if (testFrameworkIntegration == null)
                {
                    testFrameworkIntegration = new NUnitIntegration();
                }
                return testFrameworkIntegration;
            }
        }
        #endregion
    }
}