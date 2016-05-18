using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using BoDi;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure.ContextManagers;
using TechTalk.SpecFlow.Tracing;

namespace TechTalk.SpecFlow.Infrastructure
{
    using System.Collections.Concurrent;
    using System.Threading;
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
        private readonly IObjectContainer testThreadContainer;
        private readonly IInternalContextManager<FeatureContext> featureContext;
        private readonly IInternalContextManager<ScenarioContext> scenarioContext;
        private readonly IInternalContextManager<ScenarioStepContext> stepContext;
        private readonly IContainerBuilder containerBuilder;

        public ContextManager(IContainerBuilder containerBuilder,IObjectContainer testThreadContainer, IInternalContextManager<FeatureContext> featureContext, IInternalContextManager<ScenarioContext> scenarioContext, IInternalContextManager<ScenarioStepContext> stepContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.stepContext = stepContext;
            this.testThreadContainer = testThreadContainer;
            this.containerBuilder = containerBuilder;
        }

        public FeatureContext FeatureContext
        {
            get { return featureContext.Instance; }
        }

        public ScenarioContext ScenarioContext
        {
            get { return scenarioContext.Instance; }
        }

        public ScenarioStepContext StepContext 
        {
            get{return stepContext.Instance;} 
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
            var scenarioContainer = containerBuilder.CreateScenarioContainer(testThreadContainer, scenarioInfo);
            var newContext = scenarioContainer.Resolve<ScenarioContext>();
            scenarioContext.Init(newContext);
            ScenarioContext.Current = newContext;
        }

        public void CleanupScenarioContext()
        {
            scenarioContext.Cleanup();            
        }

        public void InitializeStepContext(StepInfo stepInfo)
        {
            var newContext = new ScenarioStepContext(stepInfo);
            stepContext.Init(newContext);
            ScenarioStepContext.Current = newContext;
        }

        public void CleanupStepContext()
        {
            stepContext.Cleanup();
            ScenarioStepContext.Current = stepContext.Instance;
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
            if (stepContext != null)
            {
                stepContext.Dispose();
            }
        }
    }

    
    
}
