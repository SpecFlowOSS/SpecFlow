using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    using System.Diagnostics;

    public interface IContextManager
    {
        FeatureContext FeatureContext { get; }
        ScenarioContext ScenarioContext { get; }
        ScenarioStepContext StepContext { get; }
        void InitializeFeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture);
        void CleanupFeatureContext();

        void InitializeScenarioContext(ScenarioInfo scenarioInfo);
        void CleanupScenarioContext();

        void InitializeStepContext(StepInfo stepInfo);
        void CleanupStepContext();
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
                    DisposeInstance();
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
                DisposeInstance();
            }

            private void DisposeInstance()
            {
                ((IDisposable) instance).Dispose();
                instance = null;
            }

            public void Dispose()
            {
                if (instance != null)
                {
                    DisposeInstance();
                }
            }
        }

        /// <summary>
        /// Implementation of internal context manager which keeps a stack of contexts, rather than a single one. 
        /// This allows the contexts to be used when a new context is created before the previous context has been completed 
        /// which is what happens when a step calls other steps. This means that the step contexts will be reported 
        /// correctly even when there is a nesting of steps calling steps calling steps.
        /// </summary>
        /// <typeparam name="TContext">A type derived from SpecFlowContext, which needs to be managed  in a way</typeparam>
        private class StackedInternalContextManager<TContext> : IDisposable where TContext : SpecFlowContext
        {
            private readonly ITestTracer testTracer;
            private readonly Stack<TContext> instances = new Stack<TContext>();

            public StackedInternalContextManager(ITestTracer testTracer)
            {
                this.testTracer = testTracer;
            }

            public TContext Instance
            {
                get { return instances.Any()?instances.Peek():null; }
            }

            public void Init(TContext newInstance)
            {
                instances.Push(newInstance);
            }

            public void Cleanup()
            {
                if (!instances.Any())
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was already disposed.", typeof(TContext).Name));
                    return;
                }
                var instance = instances.Pop();
                ((IDisposable)instance).Dispose();

            }

            public void Dispose()
            {
                var instance = instances.Pop();
                if (instance != null)
                {
                    ((IDisposable)instance).Dispose();

                }
            }
        }

        private readonly IObjectContainer testThreadContainer;
        private readonly InternalContextManager<ScenarioContext> scenarioContextManager;
        private readonly InternalContextManager<FeatureContext> featureContextManager;
        private readonly StackedInternalContextManager<ScenarioStepContext> stepContextManager;
        private readonly IContainerBuilder containerBuilder;

        public ContextManager(ITestTracer testTracer, IObjectContainer testThreadContainer, IContainerBuilder containerBuilder)
        {
            featureContextManager = new InternalContextManager<FeatureContext>(testTracer);
            scenarioContextManager = new InternalContextManager<ScenarioContext>(testTracer);
            stepContextManager = new StackedInternalContextManager<ScenarioStepContext>(testTracer);
            this.testThreadContainer = testThreadContainer;
            this.containerBuilder = containerBuilder;
        }

        public FeatureContext FeatureContext
        {
            get { return featureContextManager.Instance; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return scenarioContextManager.Instance; }
        }

        public ScenarioStepContext StepContext 
        {
            get{return stepContextManager.Instance;} 
        }

        public void InitializeFeatureContext(FeatureInfo featureInfo, CultureInfo bindingCulture)
        {
            var newContext = new FeatureContext(featureInfo, bindingCulture);
            featureContextManager.Init(newContext);
        }

        public void CleanupFeatureContext()
        {
            featureContextManager.Cleanup();
        }

        public void InitializeScenarioContext(ScenarioInfo scenarioInfo)
        {
            var scenarioContainer = containerBuilder.CreateScenarioContainer(testThreadContainer, scenarioInfo);
            var newContext = scenarioContainer.Resolve<ScenarioContext>();
            scenarioContextManager.Init(newContext);
        }

        public void CleanupScenarioContext()
        {
            scenarioContextManager.Cleanup();            
        }

        public void InitializeStepContext(StepInfo stepInfo)
        {
            var newContext = new ScenarioStepContext(stepInfo);
            stepContextManager.Init(newContext);
        }

        public void CleanupStepContext()
        {
            stepContextManager.Cleanup();
        }

        public void Dispose()
        {
            if (featureContextManager != null)
            {
                featureContextManager.Dispose();
            }
            if (scenarioContextManager != null)
            {
                scenarioContextManager.Dispose();
            }
            if (stepContextManager != null)
            {
                stepContextManager.Dispose();
            }
        }
    }

    
    
}
