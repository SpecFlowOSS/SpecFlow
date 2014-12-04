using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IContextManager
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        void InitializeFeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture);
        void CleanupFeatureContext();

        void InitializeScenarioContext(ScenarioInfo scenarioInfo);
        void CleanupScenarioContext();
    }

    internal static class ContextManagerExtensions
    {
        public static StepContext GetStepContext(this IContextManager contextManager)
        {
            return new StepContext(
                contextManager.FeatureContext == null ? null : contextManager.FeatureContext.FeatureInfo,
                contextManager.ScenarioContext == null ? null : contextManager.ScenarioContext.ScenarioInfo);
        }
    }

    public class ContextManager : IContextManager, IDisposable
    {
        private class InternalContextManager<TContext>: IDisposable where TContext : SpecFlowContext
        {
            private readonly ITestTracer testTracer;

            public InternalContextManager(ITestTracer testTracer)
            {
                this.testTracer = testTracer;
            }

            public TContext Instance { get; private set; }


            public void Init(TContext newInstance)
            {
                if (Instance != null)
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was not disposed.", typeof(TContext).Name));
                    Dispose();
                }
                Instance = newInstance;
            }

            public void Cleanup()
            {
                if (Instance == null)
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was already disposed.", typeof(TContext).Name));
                    return;
                }
                ((IDisposable)Instance).Dispose();
                Instance = null;
            }

            public void Dispose()
            {


                if (Instance != null)
                {
                    ((IDisposable)Instance).Dispose();
                    Instance = null;
                }
            }
        }

        private readonly IObjectContainer parentContainer;

#if BODI_LIMITEDRUNTIME  
        private InternalContextManager<ScenarioContext> _scenarioContext;
#else
        private ThreadStorage<InternalContextManager<ScenarioContext>> _scenarioContext;
#endif
        private readonly InternalContextManager<FeatureContext> featureContext;

        public ContextManager(ITestTracer testTracer, IObjectContainer parentContainer)
        {
            featureContext = new InternalContextManager<FeatureContext>(testTracer);

#if BODI_LIMITEDRUNTIME  
            _scenarioContext = new InternalContextManager<ScenarioContext>(testTracer);
#else
            _scenarioContext =new ThreadStorage<InternalContextManager<ScenarioContext>>(
                () => new InternalContextManager<ScenarioContext>(testTracer));
#endif

            this.parentContainer = parentContainer;
        }

        private InternalContextManager<ScenarioContext> scenarioContext
        {
#if BODI_LIMITEDRUNTIME  
            get{ return _scenarioContext;}
#else
            get { return _scenarioContext.ThreadInstance; }
#endif
        }

        public FeatureContext FeatureContext
        {
            get { return featureContext.Instance; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return scenarioContext.Instance; }
        }

        public void InitializeFeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture)
        {
            var newContext = new FeatureContext(featureInfo, bindingCulture);
            featureContext.Init(newContext);
            FeatureContext.Current = newContext;
        }

        public void CleanupFeatureContext()
        {
            featureContext.Cleanup();
        }

        public void InitializeScenarioContext(ScenarioInfo scenarioInfo)
        {
            var testRunner = parentContainer.Resolve<ITestRunner>(); // we need to delay-resolve the test runner to avoid circular dependencies
            var newContext = new ScenarioContext(scenarioInfo, testRunner, parentContainer);
            scenarioContext.Init(newContext);
            ScenarioContext.Current = newContext;
        }

        public void CleanupScenarioContext()
        {
            scenarioContext.Cleanup();
        }

        public void Dispose()
        {
            if (featureContext != null)
            {
                featureContext.Dispose();
            }
            if (scenarioContext != null)
            {
                scenarioContext.Dispose();
            }
        }
    }

#if ! BODI_LIMITEDRUNTIME
    public class ThreadStorage<TClass> : IDisposable
        where TClass : class
    {
        private Dictionary<int, TClass> threadInstanceDictionary = new Dictionary<int, TClass>();
        private ReaderWriterLockSlim dictionaryLock = new ReaderWriterLockSlim();
        private Func<TClass> _constructor;

        public ThreadStorage(Func<TClass> constructor)
        {
            _constructor = constructor;
        }

        public TClass ThreadInstance
        {
            get { return threadValue(); }
            set { threadValue(value); }
        }

        private TClass threadValue(TClass val)
        {
            dictionaryLock.EnterWriteLock();

            try
            {
                if (threadInstanceDictionary.ContainsKey(Thread.CurrentThread.ManagedThreadId))
                {
                    threadInstanceDictionary[Thread.CurrentThread.ManagedThreadId] = val;
                    return null;
                }
                threadInstanceDictionary.Add(Thread.CurrentThread.ManagedThreadId, val);
            }
            finally
            {
                dictionaryLock.ExitWriteLock();
            }
            return val;
        }

        private TClass threadValue()
        {
            dictionaryLock.EnterUpgradeableReadLock();
            try
            {
                TClass instance;

                if (!threadInstanceDictionary.TryGetValue(Thread.CurrentThread.ManagedThreadId, out instance))
                {
                    return threadValue(_constructor());
                }
                return instance;
            }
            finally
            {
                dictionaryLock.ExitUpgradeableReadLock();
            }    
        }

        public void Dispose()
        {
            dictionaryLock.EnterWriteLock();
            try
            {
                foreach (var instance in threadInstanceDictionary)
                {
                    if (instance.Value == null)
                        break;

                    var val = instance.Value as IDisposable;

                    if (val == null)
                        break;

                    val.Dispose();
                }

                threadInstanceDictionary.Clear();

                dictionaryLock.Dispose();
            }
            finally
            {
                dictionaryLock.ExitWriteLock();
            }

        }
    }
#endif


}
