using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using MiniDi;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IContextManager
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }

        void InitializeFeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture);
        void CleanupFeatureContext();

        void InitializeScenarioContext(ScenarioInfo scenarioInfo, ITestRunner testRunner);
        void CleanupScenarioContext();
    }

    public class ContextManager : IContextManager, IDisposable
    {
        private class InternalContextManager<TContext>: IDisposable where TContext : SpecFlowContext
        {
            private readonly ITestTracer testTracer;
            private TContext instance;

            public InternalContextManager(ITestTracer testTracer)
            {
                this.testTracer = testTracer;
            }

            public TContext Instance
            {
                get { return instance; }
            }

            public void Init(TContext newInstance)
            {
                if (instance != null)
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was not disposed.", typeof(TContext).Name));
                    Dispose();
                }
                instance = newInstance;
            }

            public void Cleanup()
            {
                if (instance == null)
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was already disposed.", typeof(TContext).Name));
                    return;
                }
                instance = null;
            }

            public void Dispose()
            {
                if (instance != null)
                {
                    ((IDisposable)instance).Dispose();
                    instance = null;
                }
            }
        }

        private readonly InternalContextManager<ScenarioContext> scenarioContext;
        private readonly InternalContextManager<FeatureContext> featureContext;

        public ContextManager(ITestTracer testTracer)
        {
            featureContext = new InternalContextManager<FeatureContext>(testTracer);
            scenarioContext = new InternalContextManager<ScenarioContext>(testTracer);
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

        public void InitializeScenarioContext(ScenarioInfo scenarioInfo, ITestRunner testRunner)
        {
            var newContext = new ScenarioContext(scenarioInfo, testRunner);
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
}
