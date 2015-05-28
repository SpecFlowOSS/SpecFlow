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
        bool IsMultiThreaded { get; }
        ITestRunner GetTestRunner(int threadId);
        void Initialize(Assembly testAssembly);
    }

    public class TestRunnerManager : ITestRunnerManager
    {
        protected readonly IObjectContainer globalContainer;
        protected readonly ITestRunContainerBuilder testRunContainerBuilder;
        protected readonly RuntimeConfiguration runtimeConfiguration;
        protected readonly IRuntimeBindingRegistryBuilder bindingRegistryBuilder;

        private Assembly testAssembly;
        private readonly Dictionary<int, ITestRunner> testRunnerRegistry = new Dictionary<int, ITestRunner>();
        private readonly object syncRoot = new object();
        private bool isTestRunInitialized;

        public bool IsMultiThreaded { get { return testRunnerRegistry.Count > 1; } }

        public TestRunnerManager(IObjectContainer globalContainer, ITestRunContainerBuilder testRunContainerBuilder, RuntimeConfiguration runtimeConfiguration, IRuntimeBindingRegistryBuilder bindingRegistryBuilder)
        {
            this.globalContainer = globalContainer;
            this.testRunContainerBuilder = testRunContainerBuilder;
            this.runtimeConfiguration = runtimeConfiguration;
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
            var bindingAssemblies = GetBindingAssemblies();
            BuildBindingRegistry(bindingAssemblies);

            testRunner.OnTestRunStart();

#if !SILVERLIGHT
            EventHandler domainUnload = delegate { OnTestRunnerEnd(); };
            AppDomain.CurrentDomain.DomainUnload += domainUnload;
            AppDomain.CurrentDomain.ProcessExit += domainUnload;
#endif
        }

        protected virtual List<Assembly> GetBindingAssemblies()
        {
            var bindingAssemblies = new List<Assembly> {testAssembly};

            var assemblyLoader = globalContainer.Resolve<IBindingAssemblyLoader>();
            bindingAssemblies.AddRange(
                runtimeConfiguration.AdditionalStepAssemblies.Select(assemblyLoader.Load));
            return bindingAssemblies;
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
            var testRunnerContainer = testRunContainerBuilder.CreateTestRunnerContainer(globalContainer);

            return testRunnerContainer.Resolve<ITestRunner>();
        }

        public void Initialize(Assembly assignedTestAssembly)
        {
            testAssembly = assignedTestAssembly;
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

        private static ITestRunnerManager GetTestRunnerManager(Assembly testAssembly, ITestRunContainerBuilder testRunContainerBuilder = null)
        {
            ITestRunnerManager testRunnerManager;
            if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out testRunnerManager))
            {
                lock (testRunnerManagerRegistrySyncRoot)
                {
                    if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out testRunnerManager))
                    {
                        testRunnerManager = CreateTestRunnerManager(testAssembly, testRunContainerBuilder);
                        testRunnerManagerRegistry.Add(testAssembly, testRunnerManager);
                    }
                }
            }
            return testRunnerManager;
        }

        private static ITestRunnerManager CreateTestRunnerManager(Assembly testAssembly, ITestRunContainerBuilder testRunContainerBuilder = null)
        {
            testRunContainerBuilder = testRunContainerBuilder ?? new TestRunContainerBuilder();

            var container = testRunContainerBuilder.CreateContainer();
            var testRunnerManager = container.Resolve<ITestRunnerManager>();
            testRunnerManager.Initialize(testAssembly); //TODO[thread-safety]: consider factory
            return testRunnerManager;
        }

        public static ITestRunner GetTestRunner(Assembly testAssembly = null, int? managedThreadId = null)
        {
            testAssembly = testAssembly ?? Assembly.GetCallingAssembly();
            managedThreadId = managedThreadId ?? Thread.CurrentThread.ManagedThreadId;
            var testRunnerManager = GetTestRunnerManager(testAssembly);
            return testRunnerManager.GetTestRunner(managedThreadId.Value);
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
                if (testRunnerManagerRegistry.ContainsKey(testRunnerManager.testAssembly))
                    testRunnerManagerRegistry.Remove(testRunnerManager.testAssembly);
            }
        }

        #endregion
    }
}