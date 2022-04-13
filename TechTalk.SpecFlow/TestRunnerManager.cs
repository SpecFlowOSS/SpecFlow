using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using BoDi;
using TechTalk.SpecFlow.Bindings.Discovery;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow
{
    public class TestRunnerManager : ITestRunnerManager
    {
        public const string TestRunStartWorkerId = "TestRunStart";

        protected readonly IObjectContainer _globalContainer;
        protected readonly IContainerBuilder _containerBuilder;
        protected readonly SpecFlowConfiguration _specFlowConfiguration;
        protected readonly IRuntimeBindingRegistryBuilder _bindingRegistryBuilder;
        protected readonly ITestTracer _testTracer;

        //TODO: Replace with ConcurrentDictionary
        private readonly Dictionary<string, ITestRunner> testRunnerRegistry = new();
        private readonly object syncRoot = new();
        public bool IsTestRunInitialized { get; private set; }
        private object disposeLockObj = null;
        private readonly object createTestRunnerLockObject = new();

        public Assembly TestAssembly { get; private set; }
        public Assembly[] BindingAssemblies { get; private set; }

        public bool IsMultiThreaded { get { return testRunnerRegistry.Count(tr => tr.Key != TestRunStartWorkerId) > 1; } }

        public TestRunnerManager(IObjectContainer globalContainer, IContainerBuilder containerBuilder, SpecFlowConfiguration specFlowConfiguration, IRuntimeBindingRegistryBuilder bindingRegistryBuilder,
            ITestTracer testTracer)
        {
            _globalContainer = globalContainer;
            _containerBuilder = containerBuilder;
            _specFlowConfiguration = specFlowConfiguration;
            _bindingRegistryBuilder = bindingRegistryBuilder;
            _testTracer = testTracer;
        }

        public virtual ITestRunner CreateTestRunner(string testWorkerId)
        {
            var testRunner = CreateTestRunnerInstance();
            testRunner.InitializeTestRunner(testWorkerId);

            lock (createTestRunnerLockObject)
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

            void DomainUnload(object sender, EventArgs e)
            {
                OnDomainUnloadAsync().Wait();
            }

            AppDomain.CurrentDomain.DomainUnload += DomainUnload;
            AppDomain.CurrentDomain.ProcessExit += DomainUnload;
        }

        protected virtual Assembly[] GetBindingAssemblies()
        {
            var bindingAssemblies = new List<Assembly> { TestAssembly };

            var assemblyLoader = _globalContainer.Resolve<IBindingAssemblyLoader>();
            bindingAssemblies.AddRange(
                _specFlowConfiguration.AdditionalStepAssemblies.Select(assemblyLoader.Load));
            return bindingAssemblies.ToArray();
        }

        protected virtual void BuildBindingRegistry(IEnumerable<Assembly> bindingAssemblies)
        {
            foreach (Assembly assembly in bindingAssemblies)
            {
                _bindingRegistryBuilder.BuildBindingsFromAssembly(assembly);
            }
            _bindingRegistryBuilder.BuildingCompleted();
        }

        protected internal virtual async Task OnDomainUnloadAsync()
        {
            await DisposeAsync();
        }

        public async Task FireTestRunEndAsync()
        {
            // this method must not be called multiple times
            var onTestRunnerEndExecutionHost = testRunnerRegistry.Values.FirstOrDefault();
            if (onTestRunnerEndExecutionHost != null)
                await onTestRunnerEndExecutionHost.OnTestRunEndAsync();
        }

        public async Task FireTestRunStartAsync()
        {
            // this method must not be called multiple times
            var onTestRunnerEndExecutionHost = testRunnerRegistry.Values.FirstOrDefault();
            if (onTestRunnerEndExecutionHost != null)
                await onTestRunnerEndExecutionHost.OnTestRunStartAsync();
        }

        protected virtual ITestRunner CreateTestRunnerInstance()
        {
            var testThreadContainer = _containerBuilder.CreateTestThreadContainer(_globalContainer);

            return testThreadContainer.Resolve<ITestRunner>();
        }

        public void Initialize(Assembly assignedTestAssembly)
        {
            TestAssembly = assignedTestAssembly;
        }

        public virtual ITestRunner GetTestRunner(string testWorkerId)
        {
            try
            {
                return GetTestRunnerWithoutExceptionHandling(testWorkerId);
            }
            catch (Exception ex)
            {
                _testTracer.TraceError(ex,TimeSpan.Zero);
                throw;
            }
        }

        private ITestRunner GetTestRunnerWithoutExceptionHandling(string testWorkerId)
        {
            if (!testRunnerRegistry.TryGetValue(testWorkerId, out var testRunner))
            {
                lock (syncRoot)
                {
                    if (!testRunnerRegistry.TryGetValue(testWorkerId, out testRunner))
                    {
                        testRunner = CreateTestRunner(testWorkerId);
                        testRunnerRegistry.Add(testWorkerId, testRunner);

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

        public virtual async ValueTask DisposeAsync()
        {
            if (Interlocked.CompareExchange<object>(ref disposeLockObj, new object(), null) == null)
            {
                await FireTestRunEndAsync();

                // this call dispose on this object, but the disposeLockObj will avoid double execution
                _globalContainer.Dispose();

                testRunnerRegistry.Clear();
                await OnTestRunnerManagerDisposedAsync(this);
            }
        }

        #region Static API

        //TODO: Replace with ConcurrentDictionary
        private static readonly Dictionary<Assembly, ITestRunnerManager> testRunnerManagerRegistry = new(1);
        private static readonly SemaphoreSlim testRunnerManagerRegistrySemaphore = new(1);

        public static async Task<ITestRunnerManager> GetTestRunnerManagerAsync(Assembly testAssembly = null, IContainerBuilder containerBuilder = null, bool createIfMissing = true)
        {
            testAssembly ??= GetCallingAssembly();

            if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out var testRunnerManager))
            {
                await testRunnerManagerRegistrySemaphore.WaitAsync();
                try
                {
                    if (!testRunnerManagerRegistry.TryGetValue(testAssembly, out testRunnerManager))
                    {
                        if (!createIfMissing)
                            return null;

                        testRunnerManager = CreateTestRunnerManager(testAssembly, containerBuilder);
                        testRunnerManagerRegistry.Add(testAssembly, testRunnerManager);
                    }
                }
                finally
                {
                    testRunnerManagerRegistrySemaphore.Release();
                }
            }
            return testRunnerManager;
        }

        /// <summary>
        /// This is a workaround method solving not correctly working Assembly.GetCallingAssembly() when called from async method (due to state machine).
        /// </summary>
        private static Assembly GetCallingAssembly([CallerMemberName] string callingMethodName = null)
        {
            var stackTrace = new StackTrace();

            var callingMethodIndex = -1;

            for (var i = 0; i < stackTrace.FrameCount; i++)
            {
                var frame = stackTrace.GetFrame(i);

                if (frame.GetMethod().Name == callingMethodName)
                {
                    callingMethodIndex = i;
                    break;
                }
            }

            Assembly result = null;

            if (callingMethodIndex >= 0 && callingMethodIndex + 1 < stackTrace.FrameCount)
            {
                result = stackTrace.GetFrame(callingMethodIndex + 1).GetMethod().DeclaringType?.Assembly;
            }

            return result ?? GetCallingAssembly();
        }
        
        private static ITestRunnerManager CreateTestRunnerManager(Assembly testAssembly, IContainerBuilder containerBuilder = null)
        {
            containerBuilder ??= new ContainerBuilder();

            var container = containerBuilder.CreateGlobalContainer(testAssembly);
            var testRunnerManager = container.Resolve<ITestRunnerManager>();
            testRunnerManager.Initialize(testAssembly);
            return testRunnerManager;
        }

        public static async Task OnTestRunEndAsync(Assembly testAssembly = null, IContainerBuilder containerBuilder = null)
        {
            testAssembly ??= GetCallingAssembly();
            var testRunnerManager = await GetTestRunnerManagerAsync(testAssembly, createIfMissing: false, containerBuilder: containerBuilder);
            if (testRunnerManager != null)
            {
                await testRunnerManager.FireTestRunEndAsync();
                await testRunnerManager.DisposeAsync();
            }
        }

        public static async Task OnTestRunStartAsync(Assembly testAssembly = null, string testWorkerId = null, IContainerBuilder containerBuilder = null)
        {
            testAssembly ??= GetCallingAssembly();
            var testRunnerManager = await GetTestRunnerManagerAsync(testAssembly, createIfMissing: true, containerBuilder: containerBuilder);
            testRunnerManager.GetTestRunner(testWorkerId ?? TestRunStartWorkerId);

            await testRunnerManager.FireTestRunStartAsync();
        }

        public static async Task<ITestRunner> GetTestRunnerAsync(string testWorkerId = null, Assembly testAssembly = null, IContainerBuilder containerBuilder = null)
        {
            testWorkerId ??= Guid.NewGuid().ToString(); //Creates a Test Runner with a unique test thread
            testAssembly ??= GetCallingAssembly();
            var testRunnerManager = await GetTestRunnerManagerAsync(testAssembly, containerBuilder);
            return testRunnerManager.GetTestRunner(testWorkerId);
        }

        internal static async Task ResetAsync()
        {
            foreach (var testRunnerManager in testRunnerManagerRegistry.Values.ToArray())
            {
                await testRunnerManager.DisposeAsync();
            }
            
            await testRunnerManagerRegistrySemaphore.WaitAsync();
            try
            {
                testRunnerManagerRegistry.Clear();
            }
            finally
            {
                testRunnerManagerRegistrySemaphore.Release();
            }
        }


        private static async Task OnTestRunnerManagerDisposedAsync(TestRunnerManager testRunnerManager)
        {
            await testRunnerManagerRegistrySemaphore.WaitAsync();
            try
            {
                if (testRunnerManagerRegistry.ContainsKey(testRunnerManager.TestAssembly))
                    testRunnerManagerRegistry.Remove(testRunnerManager.TestAssembly);
            }
            finally
            {
                testRunnerManagerRegistrySemaphore.Release();
            }
        }

        #endregion
    }
}