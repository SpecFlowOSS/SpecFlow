using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using BoDi;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow
{
    public interface ITestRunnerManager : IDisposable
    {
        Assembly TestAssembly { get; }
        Assembly[] BindingAssemblies { get; }
        bool IsMultiThreaded { get; }
        ITestRunner GetTestRunner(int threadId);
        void Initialize(Assembly testAssembly);
    }

    public class TestRunnerManager : ITestRunnerManager
    {
        protected readonly IObjectContainer globalContainer;
        protected readonly IContainerBuilder containerBuilder;
        protected readonly Configuration.SpecFlowConfiguration specFlowConfiguration;
        protected readonly IRuntimeBindingRegistryBuilder bindingRegistryBuilder;

        private readonly Dictionary<int, ITestRunner> testRunnerRegistry = new Dictionary<int, ITestRunner>();
        private readonly object syncRoot = new object();
        private bool isTestRunInitialized;

        public Assembly TestAssembly { get; private set; }
        public Assembly[] BindingAssemblies { get; private set; }

        public bool IsMultiThreaded { get { return testRunnerRegistry.Count > 1; } }

        public TestRunnerManager(IObjectContainer globalContainer, IContainerBuilder containerBuilder, Configuration.SpecFlowConfiguration specFlowConfiguration, IRuntimeBindingRegistryBuilder bindingRegistryBuilder)
        {
            this.globalContainer = globalContainer;
            this.containerBuilder = containerBuilder;
            this.specFlowConfiguration = specFlowConfiguration;
            this.bindingRegistryBuilder = bindingRegistryBuilder;
        }

        public virtual ITestRunner CreateTestRunner(int threadId)
        {
            var testRunner = CreateTestRunnerInstance();
            testRunner.InitializeTestRunner(threadId);

            lock (this)
            {
                if (!isTestRunInitialized)
                {
                    InitializeBindingRegistry(testRunner);
                    isTestRunInitialized = true;
                }
            }

            return testRunner;
        }

        protected virtual void InitializeBindingRegistry(ITestRunner testRunner)
        {
            BindingAssemblies = GetBindingAssemblies();
            BuildBindingRegistry(BindingAssemblies);

            testRunner.OnTestRunStart();

#if !SILVERLIGHT
            EventHandler domainUnload = delegate { OnTestRunnerEnd(); };
            AppDomain.CurrentDomain.DomainUnload += domainUnload;
            AppDomain.CurrentDomain.ProcessExit += domainUnload;
#endif
        }

        protected virtual Assembly[] GetBindingAssemblies()
        {
            var bindingAssemblies = new List<Assembly> { TestAssembly };

            var assemblyLoader = globalContainer.Resolve<IBindingAssemblyLoader>();
            bindingAssemblies.AddRange(
                specFlowConfiguration.AdditionalStepAssemblies.Select(assemblyLoader.Load));
            return bindingAssemblies.ToArray();
        }

        protected virtual void BuildBindingRegistry(IEnumerable<Assembly> bindingAssemblies)
        {
            foreach (Assembly assembly in bindingAssemblies)
            {
                bindingRegistryBuilder.BuildBindingsFromAssembly(assembly);
            }
            bindingRegistryBuilder.BuildingCompleted();
        }

        protected virtual void OnTestRunnerEnd()
        {
            var onTestRunnerEndExecutionHost = testRunnerRegistry.Values.FirstOrDefault();
            if (onTestRunnerEndExecutionHost != null)
                onTestRunnerEndExecutionHost.OnTestRunEnd();

            // this will dispose this object
            globalContainer.Dispose();
        }

        protected virtual ITestRunner CreateTestRunnerInstance()
        {
            var testThreadContainer = containerBuilder.CreateTestThreadContainer(globalContainer);

            return testThreadContainer.Resolve<ITestRunner>();
        }

        public void Initialize(Assembly assignedTestAssembly)
        {
            TestAssembly = assignedTestAssembly;
        }

        public virtual ITestRunner GetTestRunner(int threadId)
        {
            ITestRunner testRunner;
            if (!testRunnerRegistry.TryGetValue(threadId, out testRunner))
            {
                lock(syncRoot)
                {
                    if (!testRunnerRegistry.TryGetValue(threadId, out testRunner))
                    {
                        testRunner = CreateTestRunner(threadId);
                        testRunnerRegistry.Add(threadId, testRunner);

                        if (IsMultiThreaded)
                        {
                            FeatureContext.DisableSingletonInstance();
                            ScenarioContext.DisableSingletonInstance();
                            ScenarioStepContext.DisableSingletonInstance();
                        }
                    }
                }
            }
            return testRunner;
        }

        public virtual void Dispose()
        {
            testRunnerRegistry.Clear();
            OnTestRunnerManagerDisposed(this);
        }

        #region Static API

        private static readonly Dictionary<Assembly, ITestRunnerManager> testRunnerManagerRegistry = new Dictionary<Assembly, ITestRunnerManager>(1);
        private static readonly object testRunnerManagerRegistrySyncRoot = new object();
        private const int FixedLogicalThreadId = 0;

        private static ITestRunnerManager GetTestRunnerManager(Assembly testAssembly, IContainerBuilder containerBuilder = null)
        {
            ITestRunnerManager testRunnerManager;
            if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out testRunnerManager))
            {
                lock (testRunnerManagerRegistrySyncRoot)
                {
                    if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out testRunnerManager))
                    {
                        testRunnerManager = CreateTestRunnerManager(testAssembly, containerBuilder);
                        testRunnerManagerRegistry.Add(testAssembly, testRunnerManager);
                    }
                }
            }
            return testRunnerManager;
        }

        private static ITestRunnerManager CreateTestRunnerManager(Assembly testAssembly, IContainerBuilder containerBuilder = null)
        {
            containerBuilder = containerBuilder ?? new ContainerBuilder();

            var container = containerBuilder.CreateGlobalContainer();
            var testRunnerManager = container.Resolve<ITestRunnerManager>();
            testRunnerManager.Initialize(testAssembly);
            return testRunnerManager;
        }

        public static ITestRunner GetTestRunner(Assembly testAssembly = null, int? managedThreadId = null)
        {
            testAssembly = testAssembly ?? Assembly.GetCallingAssembly();
            managedThreadId = GetLogicalThreadId(managedThreadId);
            var testRunnerManager = GetTestRunnerManager(testAssembly);
            return testRunnerManager.GetTestRunner(managedThreadId.Value);
        }

        private static int GetLogicalThreadId(int? managedThreadId)
        {
            if (ParallelExecutionIsDisabled())
            {
                return FixedLogicalThreadId;
            }

            return managedThreadId ?? Thread.CurrentThread.ManagedThreadId;
        }

        private static bool ParallelExecutionIsDisabled()
        {
            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariableNames.NCrunch)) ||
                !string.IsNullOrEmpty(Environment.GetEnvironmentVariable(EnvironmentVariableNames.SpecflowDisableParallelExecution)))
            {
                return true;
            }

            return false;
        }

        internal static void Reset()
        {
            lock (testRunnerManagerRegistrySyncRoot)
            {
                foreach (var testRunnerManager in testRunnerManagerRegistry.Values.ToArray())
                {
                    testRunnerManager.Dispose();
                }
                testRunnerManagerRegistry.Clear();
            }
        }


        private static void OnTestRunnerManagerDisposed(TestRunnerManager testRunnerManager)
        {
            lock (testRunnerManagerRegistrySyncRoot)
            {
                if (testRunnerManagerRegistry.ContainsKey(testRunnerManager.TestAssembly))
                    testRunnerManagerRegistry.Remove(testRunnerManager.TestAssembly);
            }
        }

        #endregion
    }
}