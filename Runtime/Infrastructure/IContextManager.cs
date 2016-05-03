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
        private readonly IObjectContainer parentContainer;
        private readonly IInternalContextManager<FeatureContext> featureContext;
        private readonly IInternalContextManager<ScenarioContext> scenarioContext;
        private readonly IInternalContextManager<ScenarioStepContext> stepContext;

        public ContextManager(IObjectContainer parentContainer, IInternalContextManager<FeatureContext> featureContext, IInternalContextManager<ScenarioContext> scenarioContext, IInternalContextManager<ScenarioStepContext> stepContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
            this.stepContext = stepContext;
            this.parentContainer = parentContainer;
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
            var newContext = new ScenarioContext(scenarioInfo, parentContainer);
            SetupScenarioContainer(newContext);
            scenarioContext.Init(newContext);
            ScenarioContext.Current = newContext;
        }

        protected virtual void SetupScenarioContainer(ScenarioContext newContext)
        {
            newContext.ScenarioContainer.RegisterInstanceAs(newContext);
            newContext.ScenarioContainer.RegisterInstanceAs(FeatureContext);

            newContext.ScenarioContainer.ObjectCreated += obj =>
            {
                var containerDependentObject = obj as IContainerDependentObject;
                if (containerDependentObject != null)
                    containerDependentObject.SetObjectContainer(newContext.ScenarioContainer);
            };
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
