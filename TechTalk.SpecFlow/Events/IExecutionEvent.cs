using System;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Events
{
    public interface IExecutionEvent
    {
    }

    public class ExecutionEvent : IExecutionEvent
    {
    }

    public class TestRunStartingEvent : ExecutionEvent
    {
    }
    public class TestRunStartedEvent : ExecutionEvent
    {
    }

    public class TestRunFinishingEvent : ExecutionEvent
    {
    }
    public class TestRunFinishedEvent : ExecutionEvent
    {
    }

    public class FeatureStartedEvent : ExecutionEvent
    {
        public FeatureContext FeatureContext { get; }

        public FeatureStartedEvent(FeatureContext featureContext)
        {
            FeatureContext = featureContext;
        }
    }
    
    public class FeatureFinishedEvent : ExecutionEvent
    {
        public FeatureContext FeatureContext { get; }

        public FeatureFinishedEvent(FeatureContext featureContext)
        {
            FeatureContext = featureContext;
        }
    }
    
    public class ScenarioStartedEvent : ExecutionEvent
    {
        public FeatureContext FeatureContext { get; }

        public ScenarioContext ScenarioContext { get; }

        public ScenarioStartedEvent(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
        }
    }
    
    
    public class ScenarioFinishedEvent : ExecutionEvent
    {
        public FeatureContext FeatureContext { get; }

        public ScenarioContext ScenarioContext { get; }

        public ScenarioFinishedEvent(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
        }
    }
    
    public class StepStartedEvent : ExecutionEvent
    {
        public FeatureContext FeatureContext { get; }

        public ScenarioContext ScenarioContext { get; }

        public ScenarioStepContext StepContext { get; }

        public StepStartedEvent(FeatureContext featureContext, ScenarioContext scenarioContext, ScenarioStepContext stepContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
            StepContext = stepContext;
        }
    }
    
    public class StepFinishedEvent : ExecutionEvent
    {
        public FeatureContext FeatureContext { get; }

        public ScenarioContext ScenarioContext { get; }

        public ScenarioStepContext StepContext { get; }

        public StepFinishedEvent(FeatureContext featureContext, ScenarioContext scenarioContext, ScenarioStepContext stepContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
            StepContext = stepContext;
        }
    }
    
    
    public class ScenarioSkippedEvent : ExecutionEvent
    {}
    
    public class StepSkippedEvent : ExecutionEvent
    {}

    public class StepBindingStartedEvent : ExecutionEvent
    {
        public IStepDefinitionBinding StepDefinitionBinding { get; }

        public StepBindingStartedEvent(IStepDefinitionBinding stepDefinitionBinding)
        {
            StepDefinitionBinding = stepDefinitionBinding;
        }
    }

    public class StepBindingFinishedEvent : ExecutionEvent
    {
        public IStepDefinitionBinding StepDefinitionBinding { get; }

        public TimeSpan Duration { get; }

        public StepBindingFinishedEvent(IStepDefinitionBinding stepDefinitionBinding, TimeSpan duration)
        {
            StepDefinitionBinding = stepDefinitionBinding;
            Duration = duration;
        }
    }

    public class HookBindingStartedEvent : ExecutionEvent
    {
        public IHookBinding HookBinding { get; }

        public HookBindingStartedEvent(IHookBinding hookBinding)
        {
            HookBinding = hookBinding;
        }
    }
    
    public class HookBindingFinishedEvent : ExecutionEvent
    {
        public IHookBinding HookBinding { get; }

        public TimeSpan Duration { get; }

        public HookBindingFinishedEvent(IHookBinding hookBinding, TimeSpan duration)
        {
            HookBinding = hookBinding;
            Duration = duration;
        }
    }

    public class OutputAddedEvent : ExecutionEvent
    {
        public string Text { get; }

        public OutputAddedEvent(string text)
        {
            Text = text;
        }
    }

    public class AttachmentAddedEvent : ExecutionEvent
    {
        public string FileName { get; }

        public AttachmentAddedEvent(string fileName)
        {
            FileName = fileName;
        }
    }
}

