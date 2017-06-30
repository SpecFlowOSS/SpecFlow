using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    public class ContextManager : IContextManager, IDisposable
    {
        private class InternalContextManager<TContext>: IDisposable where TContext : SpecFlowContext
        {
            private readonly ITestTracer testTracer;
            private TContext instance;
            private IObjectContainer objectContainer;

            public InternalContextManager(ITestTracer testTracer)
            {
                this.testTracer = testTracer;
            }

            public TContext Instance
            {
                get { return instance; }
            }

            public void Init(TContext newInstance, IObjectContainer newObjectContainer)
            {
                if (instance != null)
                {
                    testTracer.TraceWarning(string.Format("The previous {0} was not disposed.", typeof(TContext).Name));
                    DisposeInstance();
                }
                instance = newInstance;
                objectContainer = newObjectContainer;
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
                objectContainer.Dispose();
                instance = null;
                objectContainer = null;
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
                get { return IsEmpty ? null : instances.Peek(); }
            }

            public bool IsEmpty => !instances.Any();

            public void Push(TContext newInstance)
            {
                instances.Push(newInstance);                
            }

            public void RemoveTop()
            {
                if (IsEmpty)
                {
                    testTracer.TraceWarning($"The previous {typeof(TContext).Name} was already disposed.");
                    return;
                }
                var instance = instances.Pop();
                ((IDisposable)instance).Dispose();
            }

            public void Dispose()
            {
                Reset();
            }

            public void Reset()
            {
                while (!IsEmpty)
                {
                    RemoveTop();
                }
            }
        }

        private readonly IObjectContainer testThreadContainer;
        private readonly InternalContextManager<ScenarioContext> scenarioContextManager;
        private readonly InternalContextManager<FeatureContext> featureContextManager;
        private readonly StackedInternalContextManager<ScenarioStepContext> stepContextManager;
        private readonly IContainerBuilder containerBuilder;

        /// <summary>
        /// Holds the StepDefinitionType of the last step that was executed from the actual featrure file, excluding the types of the steps that were executed during the calling of a step
        /// </summary>
        public StepDefinitionType? CurrentTopLevelStepDefinitionType { get; private set; }

        public ContextManager(ITestTracer testTracer, IObjectContainer testThreadContainer, IContainerBuilder containerBuilder)
        {
            this.featureContextManager = new InternalContextManager<FeatureContext>(testTracer);
            this.scenarioContextManager = new InternalContextManager<ScenarioContext>(testTracer);
            this.stepContextManager = new StackedInternalContextManager<ScenarioStepContext>(testTracer);
            this.testThreadContainer = testThreadContainer;
            this.containerBuilder = containerBuilder;

            InitializeTestThreadContext();
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

        public TestThreadContext TestThreadContext { get; private set; }

        private void InitializeTestThreadContext()
        {
            // Since both TestThreadContext and ContextManager are in the same container (test thread container)
            // their lifetime is the same, so we do not need the swop infrastructure like for the other contexts.
            // We just neet to initliaze it during contstructuion time.
            var testThreadContext = testThreadContainer.Resolve<TestThreadContext>();
            this.TestThreadContext = testThreadContext;
        }

        public void InitializeFeatureContext(FeatureInfo featureInfo)
        {
            var featureContainer = containerBuilder.CreateFeatureContainer(testThreadContainer, featureInfo);
            var newContext = featureContainer.Resolve<FeatureContext>();
            featureContextManager.Init(newContext, featureContainer);
            FeatureContext.Current = newContext;
        }

        public void CleanupFeatureContext()
        {
            featureContextManager.Cleanup();
        }

        public void InitializeScenarioContext(ScenarioInfo scenarioInfo)
        {
            var scenarioContainer = containerBuilder.CreateScenarioContainer(FeatureContext.FeatureContainer, scenarioInfo);
            var newContext = scenarioContainer.Resolve<ScenarioContext>();
            scenarioContextManager.Init(newContext, scenarioContainer);
            ScenarioContext.Current = newContext;

            ResetCurrentStepStack();
        }

        private void ResetCurrentStepStack()
        {
            stepContextManager.Reset();
            CurrentTopLevelStepDefinitionType = null;
            ScenarioStepContext.Current = null;
        }

        public void CleanupScenarioContext()
        {
            scenarioContextManager.Cleanup();            
        }

        public void InitializeStepContext(StepInfo stepInfo)
        {
            if (stepContextManager.IsEmpty) // top-level step comes
                CurrentTopLevelStepDefinitionType = stepInfo.StepDefinitionType;
            var newContext = new ScenarioStepContext(stepInfo);
            stepContextManager.Push(newContext);
            ScenarioStepContext.Current = newContext;
        }

        public void CleanupStepContext()
        {
            stepContextManager.RemoveTop();
            ScenarioStepContext.Current = stepContextManager.Instance;
            // we do not reset CurrentTopLevelStepDefinitionType in order to "remember" last top level type for And and But steps
        }

        public void Dispose()
        {
            featureContextManager?.Dispose();
            scenarioContextManager?.Dispose();
            stepContextManager?.Dispose();
        }
    }
}