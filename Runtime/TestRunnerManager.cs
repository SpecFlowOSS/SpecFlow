using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
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
        private readonly ITestRunnerFactory testRunnerFactory;
        private Assembly testAssembly;

        public TestRunnerManager(ITestRunnerFactory testRunnerFactory)
        {
            this.testRunnerFactory = testRunnerFactory;
        }

        private readonly Dictionary<int, ITestRunner> testRunnerRegistry = new Dictionary<int, ITestRunner>();

        private readonly object syncRoot = new object();

        public virtual ITestRunner CreateTestRunner()
        {
            return testRunnerFactory.Create(testAssembly);
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