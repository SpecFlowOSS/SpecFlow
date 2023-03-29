using System;
using System.Collections.Concurrent;
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

        private readonly ConcurrentDictionary<string, ITestRunner> _testRunnerRegistry = new();
        public bool IsTestRunInitialized { get; private set; }
        private int _wasDisposed = 0;
        private int _wasSingletonInstanceDisabled = 0;
        private readonly object createTestRunnerLockObject = new();

        public Assembly TestAssembly { get; private set; }
        public Assembly[] BindingAssemblies { get; private set; }

        public bool IsMultiThreaded => GetWorkerTestRunnerCount() > 1;

        public TestRunnerManager(IObjectContainer globalContainer, IContainerBuilder containerBuilder, SpecFlowConfiguration specFlowConfiguration, IRuntimeBindingRegistryBuilder bindingRegistryBuilder,
            ITestTracer testTracer)
        {
            _globalContainer = globalContainer;
            _containerBuilder = containerBuilder;
            _specFlowConfiguration = specFlowConfiguration;
            _bindingRegistryBuilder = bindingRegistryBuilder;
            _testTracer = testTracer;
        }

        private int GetWorkerTestRunnerCount()
        {
            var hasTestRunStartWorker = _testRunnerRegistry.ContainsKey(TestRunStartWorkerId);
            return _testRunnerRegistry.Count - (hasTestRunStartWorker ? 1 : 0);
        }

        public virtual ITestRunner CreateTestRunner(string testWorkerId = "default-worker")
        {
            var testRunner = CreateTestRunnerInstance();
            testRunner.InitializeTestRunner(testWorkerId);

            if (!IsTestRunInitialized)
            {
                lock (createTestRunnerLockObject)
                {
                    if (!IsTestRunInitialized)
                    {
                        InitializeBindingRegistry(testRunner);
                        IsTestRunInitialized = true;
                    }
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
            var onTestRunnerEndExecutionHost = _testRunnerRegistry.Values.FirstOrDefault();
            if (onTestRunnerEndExecutionHost != null)
                await onTestRunnerEndExecutionHost.OnTestRunEndAsync();
        }

        public async Task FireTestRunStartAsync()
        {
            // this method must not be called multiple times
            var onTestRunnerStartExecutionHost = _testRunnerRegistry.Values.FirstOrDefault();
            if (onTestRunnerStartExecutionHost != null)
                await onTestRunnerStartExecutionHost.OnTestRunStartAsync();
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
            testWorkerId ??= Guid.NewGuid().ToString(); //Creates a Test Runner with a unique test thread
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
            if (testWorkerId == null)
                throw new ArgumentNullException(nameof(testWorkerId));

            bool wasAdded = false;

            var testRunner = _testRunnerRegistry.GetOrAdd(
                testWorkerId,
                workerId =>
                {
                    wasAdded = true;
                    return CreateTestRunner(workerId);
                });

            if (wasAdded && IsMultiThreaded && Interlocked.CompareExchange(ref _wasSingletonInstanceDisabled, 1, 0) == 0)
            {
                FeatureContext.DisableSingletonInstance();
                ScenarioContext.DisableSingletonInstance();
                ScenarioStepContext.DisableSingletonInstance();
            }

            return testRunner;
        }

        public virtual async Task DisposeAsync()
        {
            if (Interlocked.CompareExchange(ref _wasDisposed, 1, 0) == 0)
            {
                await FireTestRunEndAsync();

                // this call dispose on this object, but the disposeLockObj will avoid double execution
                _globalContainer.Dispose();

                _testRunnerRegistry.Clear();
                OnTestRunnerManagerDisposed(this);
            }
        }

        #region Static API

        private static readonly ConcurrentDictionary<Assembly, ITestRunnerManager> _testRunnerManagerRegistry = new();

        public static ITestRunnerManager GetTestRunnerManager(Assembly testAssembly = null, IContainerBuilder containerBuilder = null, bool createIfMissing = true)
        {
            testAssembly ??= GetCallingAssembly();

            if (!createIfMissing)
            {
                return _testRunnerManagerRegistry.TryGetValue(testAssembly, out var value) ? value : null;
            }

            var testRunnerManager = _testRunnerManagerRegistry.GetOrAdd(
                testAssembly,
                assembly => CreateTestRunnerManager(assembly, containerBuilder));
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
            var testRunnerManager = GetTestRunnerManager(testAssembly, createIfMissing: false, containerBuilder: containerBuilder);
            if (testRunnerManager != null)
            {
                await testRunnerManager.FireTestRunEndAsync();
                await testRunnerManager.DisposeAsync();
            }
        }

        public static async Task OnTestRunStartAsync(Assembly testAssembly = null, string testWorkerId = null, IContainerBuilder containerBuilder = null)
        {
            testAssembly ??= GetCallingAssembly();
            var testRunnerManager = GetTestRunnerManager(testAssembly, createIfMissing: true, containerBuilder: containerBuilder);
            testRunnerManager.GetTestRunner(testWorkerId ?? TestRunStartWorkerId);

            await testRunnerManager.FireTestRunStartAsync();
        }

        public static ITestRunner GetTestRunnerForAssembly(Assembly testAssembly = null, string testWorkerId = null, IContainerBuilder containerBuilder = null)
        {
            testAssembly ??= GetCallingAssembly();
            var testRunnerManager = GetTestRunnerManager(testAssembly, containerBuilder);
            return testRunnerManager.GetTestRunner(testWorkerId);
        }

        internal static async Task ResetAsync()
        {
            while (!_testRunnerManagerRegistry.IsEmpty)
            {
                foreach (var assembly in _testRunnerManagerRegistry.Keys.ToArray())
                {
                    if (_testRunnerManagerRegistry.TryRemove(assembly, out var testRunnerManager))
                    {
                        await testRunnerManager.DisposeAsync();
                    }
                }
            }
        }

        private static void OnTestRunnerManagerDisposed(TestRunnerManager testRunnerManager)
        {
            _testRunnerManagerRegistry.TryRemove(testRunnerManager.TestAssembly, out _);
        }

        #endregion
    }
}