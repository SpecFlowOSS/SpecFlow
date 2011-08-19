using System;
using System.Reflection;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

namespace TechTalk.SpecFlow
{
    /// <summary>
    /// A mini IoC container to access the well-known objects.
    /// </summary>
    /// <remarks>
    /// We do not use an external DI tool, because it could cause a version conflict with 
    /// the DI tool used by the tested application.
    /// </remarks>
    internal class ObjectContainer
    {
        #region TestRunner
        private static ITestRunner syncTestRunner;
        private static ITestRunner asyncTestRunner;

        internal static ITestRunner SyncTestRunner
        {
            get { return EnsureSyncTestRunner(Assembly.GetCallingAssembly()); }
            set { syncTestRunner = value; }
        }

        internal static ITestRunner EnsureSyncTestRunner(Assembly callingAssembly)
        {
            return GetOrCreate(ref syncTestRunner,
                               delegate
                                   {
                                       var container = TestRunContainerBuilder.CreateContainer();
                                       var factory = container.Resolve<ITestRunnerFactory>();
                                       return factory.Create(callingAssembly);
                                   });
        }

        internal static ITestRunner AsyncTestRunner
        {
            get { return EnsureAsyncTestRunner(Assembly.GetCallingAssembly()); }
            set { asyncTestRunner = value; }
        }

        internal static ITestRunner EnsureAsyncTestRunner(Assembly callingAssembly)
        {
            return GetOrCreate(ref asyncTestRunner,
                               delegate
                               {
                                   var container = TestRunContainerBuilder.CreateContainer();
                                   container.RegisterTypeAs<AsyncTestRunnerFactory, ITestRunnerFactory>(); //TODO: better support this in the DI container
                                   var factory = container.Resolve<ITestRunnerFactory>();
                                   return factory.Create(callingAssembly);
                               });
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
                        TestTracer.TraceWarning("The previous feature context was not disposed.");
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
                        TestTracer.TraceWarning("The previous scenario context was not disposed.");
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
        private static ITestTracer testTracer = null;

        [Obsolete("Use DI")]
        private static ITestTracer TestTracer
        {
            get
            {
                return GetOrCreate(ref testTracer, typeof(TestTracer));
            }
        }
        #endregion

        #region factory helper methods
        private static TInterface GetOrCreate<TInterface>(ref TInterface storage, Type implementationType) where TInterface : class
        {
            return GetOrCreate(ref storage, () => ConfigurationServices.CreateInstance<TInterface>(implementationType));
        }

        private static TInterface GetOrCreate<TInterface>(ref TInterface storage, Func<TInterface> factory) where TInterface : class
        {
            if (storage == null)
            {
                storage = factory();
            }
            return storage;
        }

        #endregion

        internal static void Reset()
        {
            foreach (var fieldInfo in typeof(ObjectContainer).GetFields(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
            {
                fieldInfo.SetValue(null, null);
            }
        }
    }
}