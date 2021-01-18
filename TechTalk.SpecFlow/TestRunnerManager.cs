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
        void FireTestRunEnd();
        void FireTestRunStart();
    }

    public class TestRunnerManager : ITestRunnerManager
    {
        protected readonly IObjectContainer globalContainer;
        protected readonly IContainerBuilder containerBuilder;
        protected readonly Configuration.SpecFlowConfiguration specFlowConfiguration;
        protected readonly IRuntimeBindingRegistryBuilder bindingRegistryBuilder;

        private readonly ITestTracer testTracer;
        private readonly Dictionary<int, ITestRunner> testRunnerRegistry = new Dictionary<int, ITestRunner>();
        private readonly object syncRoot = new object();
        public bool IsTestRunInitialized { get; private set; }
        private object disposeLockObj = null;

        public Assembly TestAssembly { get; private set; }
        public Assembly[] BindingAssemblies { get; private set; }

        public bool IsMultiThreaded { get { return testRunnerRegistry.Count > 1; } }

        public TestRunnerManager(IObjectContainer globalContainer, IContainerBuilder containerBuilder, Configuration.SpecFlowConfiguration specFlowConfiguration, IRuntimeBindingRegistryBuilder bindingRegistryBuilder,
            ITestTracer testTracer)
        {
            this.globalContainer = globalContainer;
            this.containerBuilder = containerBuilder;
            this.specFlowConfiguration = specFlowConfiguration;
            this.bindingRegistryBuilder = bindingRegistryBuilder;
            this.testTracer = testTracer;
        }

        public virtual ITestRunner CreateTestRunner(int threadId)
        {
            var testRunner = CreateTestRunnerInstance();
            testRunner.InitializeTestRunner(threadId);

            lock (this)
            {
                if (!IsTestRunInitialized)
                {
                    InitializeBindingRegistry(testRunner);
                    IsTestRunInitialized = true;
                }
            }

            return testRunner;
        }

        protected virtual void InitializeBindingRegistry(ITestRunner testRunner)
        {
            BindingAssemblies = GetBindingAssemblies();
            BuildBindingRegistry(BindingAssemblies);

            EventHandler domainUnload = delegate { OnDomainUnload(); };
            AppDomain.CurrentDomain.DomainUnload += domainUnload;
            AppDomain.CurrentDomain.ProcessExit += domainUnload;
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

        protected internal virtual void OnDomainUnload()
        {
            Dispose();
        }

        public void FireTestRunEnd()
        {
            // this method must not be called multiple times
            var onTestRunnerEndExecutionHost = testRunnerRegistry.Values.FirstOrDefault();
            if (onTestRunnerEndExecutionHost != null)
                onTestRunnerEndExecutionHost.OnTestRunEnd();
        }

        public void FireTestRunStart()
        {
            // this method must not be called multiple times
            var onTestRunnerEndExecutionHost = testRunnerRegistry.Values.FirstOrDefault();
            if (onTestRunnerEndExecutionHost != null)
                onTestRunnerEndExecutionHost.OnTestRunStart();
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
            try
            {
                return GetTestRunnerWithoutExceptionHandling(threadId);

            }
            catch (Exception ex)
            {
                testTracer.TraceError(ex,TimeSpan.Zero);
                throw;
            }
        }

        private ITestRunner GetTestRunnerWithoutExceptionHandling(int threadId)
        {
            ITestRunner testRunner;
            if (!testRunnerRegistry.TryGetValue(threadId, out testRunner))
            {
                lock (syncRoot)
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
            if (Interlocked.CompareExchange<object>(ref disposeLockObj, new object(), null) == null)
            {
                FireTestRunEnd();

                // this call dispose on this object, but the disposeLockObj will avoid double execution
                globalContainer.Dispose();

                testRunnerRegistry.Clear();
                OnTestRunnerManagerDisposed(this);
            }
        }

        #region Static API

        private static readonly Dictionary<Assembly, ITestRunnerManager> testRunnerManagerRegistry = new Dictionary<Assembly, ITestRunnerManager>(1);
        private static readonly object testRunnerManagerRegistrySyncRoot = new object();
        private const int FixedLogicalThreadId = 0;

        public static ITestRunnerManager GetTestRunnerManager(Assembly testAssembly = null, IContainerBuilder containerBuilder = null, bool createIfMissing = true)
        {
            testAssembly = testAssembly ?? Assembly.GetCallingAssembly();

            if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out var testRunnerManager))
            {
                lock (testRunnerManagerRegistrySyncRoot)
                {
                    if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out testRunnerManager))
                    {
                        if (!createIfMissing)
                            return null;

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

            var container = containerBuilder.CreateGlobalContainer(testAssembly);
            var testRunnerManager = container.Resolve<ITestRunnerManager>();
            testRunnerManager.Initialize(testAssembly);
            return testRunnerManager;
        }

        public static void OnTestRunEnd(Assembly testAssembly = null, IContainerBuilder containerBuilder = null)
        {
            testAssembly = testAssembly ?? Assembly.GetCallingAssembly();
            var testRunnerManager = GetTestRunnerManager(testAssembly, createIfMissing: false, containerBuilder: containerBuilder);
            testRunnerManager?.FireTestRunEnd();
            testRunnerManager?.Dispose();
        }

        public static void OnTestRunStart(Assembly testAssembly = null, IContainerBuilder containerBuilder = null)
        {
            testAssembly = testAssembly ?? Assembly.GetCallingAssembly();
            var testRunnerManager = GetTestRunnerManager(testAssembly, createIfMissing: true, containerBuilder: containerBuilder);
            testRunnerManager.GetTestRunner(GetLogicalThreadId(null));

            testRunnerManager?.FireTestRunStart();
        }

        public static ITestRunner GetTestRunner(Assembly testAssembly = null, int? managedThreadId = null, IContainerBuilder containerBuilder = null)
        {
            testAssembly = testAssembly ?? Assembly.GetCallingAssembly();
            managedThreadId = GetLogicalThreadId(managedThreadId);
            var testRunnerManager = GetTestRunnerManager(testAssembly, containerBuilder);
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