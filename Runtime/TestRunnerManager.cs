using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using BoDi;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public interface ITestRunnerManager : IDisposable
    {
        ITestRunner GetTestRunner(int managedThreadId);
        void Initialize(Assembly testAssembly);
    }

    public class TestRunnerManager : ITestRunnerManager
    {
        protected readonly IObjectContainer globalContainer;
        protected readonly ITestRunContainerBuilder testRunContainerBuilder;
        protected readonly RuntimeConfiguration runtimeConfiguration;
        private Assembly testAssembly;

        public TestRunnerManager(IObjectContainer globalContainer, ITestRunContainerBuilder testRunContainerBuilder, RuntimeConfiguration runtimeConfiguration)
        {
            this.globalContainer = globalContainer;
            this.testRunContainerBuilder = testRunContainerBuilder;
            this.runtimeConfiguration = runtimeConfiguration;
        }

        private readonly Dictionary<int, ITestRunner> testRunnerRegistry = new Dictionary<int, ITestRunner>();

        private readonly object syncRoot = new object();

        public virtual ITestRunner CreateTestRunner()
        {
            var testRunner = CreateTestRunnerInstance();

            var bindingAssemblies = new List<Assembly> { testAssembly };

            var assemblyLoader = globalContainer.Resolve<IBindingAssemblyLoader>();
            bindingAssemblies.AddRange(
                runtimeConfiguration.AdditionalStepAssemblies.Select(assemblyLoader.Load));

            testRunner.InitializeTestRunner(bindingAssemblies.ToArray());

            return testRunner;
        }

        protected virtual ITestRunner CreateTestRunnerInstance()
        {
            var testRunnerContainer = testRunContainerBuilder.CreateTestRunnerContainer(globalContainer);

            return testRunnerContainer.Resolve<ITestRunner>();
        }

        public void Initialize(Assembly assignedTestAssembly)
        {
            this.testAssembly = assignedTestAssembly;
        }

        public virtual ITestRunner GetTestRunner(int managedThreadId)
        {
            ITestRunner testRunner;
            if (!testRunnerRegistry.TryGetValue(managedThreadId, out testRunner))
            {
                lock(syncRoot)
                {
                    if (!testRunnerRegistry.TryGetValue(managedThreadId, out testRunner))
                    {
                        testRunner = CreateTestRunner();
                        testRunnerRegistry.Add(managedThreadId, testRunner);
                    }
                }
            }
            return testRunner;
        }

        public virtual void Dispose()
        {
            testRunnerRegistry.Clear();
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
                foreach (var testRunnerManager in testRunnerManagerRegistry.Values)
                {
                    testRunnerManager.Dispose();
                }
                testRunnerManagerRegistry.Clear();
            }
        }

        #endregion
    }
}