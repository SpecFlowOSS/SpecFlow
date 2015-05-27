using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using TechTalk.SpecFlow.Async;
using TechTalk.SpecFlow.Infrastructure;

namespace TechTalk.SpecFlow
{
    public interface ITestRunnerManager
    {
        ITestRunner CreateTestRunner(Assembly testAssembly, bool async);
        ITestRunner GetTestRunner(Assembly testAssembly, bool async, int managedThreadId);
    }

    public class TestRunnerManager : ITestRunnerManager, IDisposable
    {
        public static ITestRunnerManager Instance { get; internal set; }

        static TestRunnerManager()
        {
            Instance = new TestRunnerManager();
        }

        private readonly ITestRunContainerBuilder testRunContainerBuilder;

        public TestRunnerManager(ITestRunContainerBuilder testRunContainerBuilder = null)
        {
            this.testRunContainerBuilder = testRunContainerBuilder ?? new TestRunContainerBuilder();
        }

        protected class TestRunnerKey
        {
            public readonly Assembly TestAssembly;
            public readonly bool Async;
            private readonly int managedThreadId;

            public TestRunnerKey(Assembly testAssembly, bool async, int managedThreadId)
            {
                TestAssembly = testAssembly;
                Async = async;
                this.managedThreadId = managedThreadId;
            }

            public bool Equals(TestRunnerKey other)
            {
                if (ReferenceEquals(null, other)) return false;
                if (ReferenceEquals(this, other)) return true;
                return Equals(other.TestAssembly, TestAssembly) && other.Async.Equals(Async) && managedThreadId == other.managedThreadId;
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != typeof (TestRunnerKey)) return false;
                return Equals((TestRunnerKey) obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = (TestAssembly != null ? TestAssembly.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ Async.GetHashCode();
                    hashCode = (hashCode*397) ^ managedThreadId;
                    return hashCode;
                }
            }
        }

        private readonly Dictionary<TestRunnerKey, ITestRunner> testRunnerRegistry = new Dictionary<TestRunnerKey, ITestRunner>();

        private readonly object syncRoot = new object();

        public ITestRunner CreateTestRunner(Assembly testAssembly, bool async)
        {
            return CreateTestRunner(new TestRunnerKey(testAssembly, async, Thread.CurrentThread.ManagedThreadId));
        }

        protected virtual ITestRunner CreateTestRunner(TestRunnerKey key)
        {
            var container = testRunContainerBuilder.CreateContainer();
            if (key.Async)
            {
                //TODO: better support this in the DI container
                container.RegisterTypeAs<AsyncTestRunner, ITestRunner>();
            }
            var factory = container.Resolve<ITestRunnerFactory>();
            return factory.Create(key.TestAssembly);
        }

        public ITestRunner GetTestRunner(Assembly testAssembly, bool async, int managedThreadId)
        {
            return GetTestRunner(new TestRunnerKey(testAssembly, async, managedThreadId));
        }

        protected virtual ITestRunner GetTestRunner(TestRunnerKey key)
        {
            ITestRunner testRunner;
            if (!testRunnerRegistry.TryGetValue(key, out testRunner))
            {
                lock(syncRoot)
                {
                    if (!testRunnerRegistry.TryGetValue(key, out testRunner))
                    {
                        testRunner = CreateTestRunner(key);
                        testRunnerRegistry.Add(key, testRunner);
                    }
                }
            }
            return testRunner;
        }

        public virtual void Dispose()
        {
            testRunnerRegistry.Clear();
        }

        #region Static Methods

        public static ITestRunner GetTestRunner()
        {
            return Instance.GetTestRunner(Assembly.GetCallingAssembly(), false, Thread.CurrentThread.ManagedThreadId);
        }

        public static ITestRunner GetAsyncTestRunner()
        {
            return Instance.GetTestRunner(Assembly.GetCallingAssembly(), true, Thread.CurrentThread.ManagedThreadId);
        }

        internal static void Reset()
        {
            if (Instance is IDisposable)
                ((IDisposable)Instance).Dispose();
            Instance = new TestRunnerManager();
        }

        #endregion
    }
}