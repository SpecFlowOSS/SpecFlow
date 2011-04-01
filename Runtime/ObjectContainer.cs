using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;
using System.Linq;

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
        #region Configuration
        private static RuntimeConfiguration configuration = null;

        public static RuntimeConfiguration Configuration
        {
            get
            {
                return GetOrCreate(ref configuration, GetConfiguration);
            }
        }

        private static RuntimeConfiguration GetConfiguration()
        {
            return RuntimeConfiguration.GetConfig();
        }
        #endregion

        #region TestRunner
        private static ITestRunner testRunner = null;

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
            return GetOrCreate(ref testRunner,
                delegate
                {
                    var result = new TestRunner();

                    List<Assembly> bindingAssemblies = new List<Assembly>();
                    bindingAssemblies.Add(callingAssembly);

                    bindingAssemblies.AddRange(configuration.AdditionalStepAssemblies);

                    result.InitializeTestRunner(bindingAssemblies.ToArray());

                    return result;
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

        #region TraceListener
        private static ITraceListener traceListener = null;

        public static ITraceListener TraceListener
        {
            get
            {
                return GetOrCreate(ref traceListener, Configuration.TraceListenerType);
            }
        }
        #endregion

        #region TestTracer
        private static ITestTracer testTracer = null;

        public static ITestTracer TestTracer
        {
            get
            {
                return GetOrCreate(ref testTracer, typeof(TestTracer));
            }
            internal set
            {
                testTracer = value;
            }
        }
        #endregion

        #region ErrorProvider
        private static ErrorProvider errorProvider = null;

        public static ErrorProvider ErrorProvider
        {
            get
            {
                return GetOrCreate(ref errorProvider);
            }
        }
        #endregion

        #region StepFormatter
        private static IStepFormatter stepFormatter = null;

        public static IStepFormatter StepFormatter
        {
            get
            {
                return GetOrCreate(ref stepFormatter, typeof(StepFormatter));
            }
            internal set
            {
                stepFormatter = value;
            }
        }
        #endregion

        #region StepDefinitionSkeletonProviderCS
        private static IStepDefinitionSkeletonProvider _stepDefinitionSkeletonProviderCS = null;
        private static IStepDefinitionSkeletonProvider _stepDefinitionSkeletonProviderVB = null;
        public static IStepDefinitionSkeletonProvider StepDefinitionSkeletonProvider(ProgrammingLanguage targetLanguage)
        {
            switch (targetLanguage)
            {
                case ProgrammingLanguage.VB:
                    return GetOrCreate(ref _stepDefinitionSkeletonProviderVB, typeof(StepDefinitionSkeletonProviderVB));
                default:
                    return GetOrCreate(ref _stepDefinitionSkeletonProviderCS, typeof(StepDefinitionSkeletonProviderCS));
            }
        }
        #endregion

        #region StepArgumentTypeConverter
        private static IStepArgumentTypeConverter stepArgumentTypeConverter = null;

        public static IStepArgumentTypeConverter StepArgumentTypeConverter
        {
            get
            {
                return GetOrCreate(ref stepArgumentTypeConverter, typeof(StepArgumentTypeConverter));
            }
            internal set
            {
                stepArgumentTypeConverter = value;
            }
        }
        #endregion

        #region UnitTestRuntimeProvider
        private static IUnitTestRuntimeProvider unitTestRuntimeProvider = null;

        public static IUnitTestRuntimeProvider UnitTestRuntimeProvider
        {
            get
            {
                return GetOrCreate(ref unitTestRuntimeProvider, Configuration.RuntimeUnitTestProviderType);
            }
        }

        #endregion

        #region BindingRegistry
        private static BindingRegistry bindingRegistry = null;

        public static BindingRegistry BindingRegistry
        {
            get
            {
                return GetOrCreate(ref bindingRegistry);
            }
            internal set
            {
                bindingRegistry = value;
            }
        }

        #endregion


        #region factory helper methods
        private static TInterface GetOrCreate<TInterface>(ref TInterface storage, Type implementationType) where TInterface : class
        {
            return GetOrCreate(ref storage, () => ConfigurationServices.CreateInstance<TInterface>(implementationType));
        }

        private static TClass GetOrCreate<TClass>(ref TClass storage) where TClass : class, new()
        {
            return GetOrCreate(ref storage, () => new TClass());
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