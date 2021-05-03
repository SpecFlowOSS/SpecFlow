using System;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Events
{
    public class ExecutionEvent : IExecutionEvent
    {
    }

    public class TestRunStartedEvent : ExecutionEvent
    {
    }

    public class TestRunFinishedEvent : ExecutionEvent
    {
    }

    public class FeatureStartedEvent : ExecutionEvent
    {
        public IFeatureContext FeatureContext { get; }

        public FeatureStartedEvent(IFeatureContext featureContext)
        {
            FeatureContext = featureContext;
        }
    }

    public class FeatureFinishedEvent : ExecutionEvent
    {
        public IFeatureContext FeatureContext { get; }

        public FeatureFinishedEvent(IFeatureContext featureContext)
        {
            FeatureContext = featureContext;
        }
    }

    public class ScenarioStartedEvent : ExecutionEvent
    {
        public IFeatureContext FeatureContext { get; }

        public IScenarioContext ScenarioContext { get; }

        public ScenarioStartedEvent(IFeatureContext featureContext, IScenarioContext scenarioContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
        }
    }


    public class ScenarioFinishedEvent : ExecutionEvent
    {
        public IFeatureContext FeatureContext { get; }

        public IScenarioContext ScenarioContext { get; }

        public ScenarioFinishedEvent(IFeatureContext featureContext, IScenarioContext scenarioContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
        }
    }

    public class StepStartedEvent : ExecutionEvent
    {
        public IFeatureContext FeatureContext { get; }

        public IScenarioContext ScenarioContext { get; }

        public IScenarioStepContext StepContext { get; }

        public StepStartedEvent(IFeatureContext featureContext, IScenarioContext scenarioContext, IScenarioStepContext stepContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
            StepContext = stepContext;
        }
    }

    public class StepFinishedEvent : ExecutionEvent
    {
        public IFeatureContext FeatureContext { get; }

        public IScenarioContext ScenarioContext { get; }

        public IScenarioStepContext StepContext { get; }

        public StepFinishedEvent(IFeatureContext featureContext, IScenarioContext scenarioContext, IScenarioStepContext stepContext)
        {
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
            StepContext = stepContext;
        }
    }

    public class HookStartedEvent : ExecutionEvent
    {
        public HookType HookType { get; }

        public IFeatureContext FeatureContext { get; }

        public IScenarioContext ScenarioContext { get; }

        public IScenarioStepContext StepContext { get; }

        public HookStartedEvent(HookType hookType, IFeatureContext featureContext, IScenarioContext scenarioContext, IScenarioStepContext stepContext)
        {
            HookType = hookType;
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
            StepContext = stepContext;
        }
    }

    public class HookFinishedEvent : ExecutionEvent
    {
        public HookType HookType { get; }

        public IFeatureContext FeatureContext { get; }

        public IScenarioContext ScenarioContext { get; }

        public IScenarioStepContext StepContext { get; }

        public Exception HookException { get; }

        public HookFinishedEvent(HookType hookType, IFeatureContext featureContext, IScenarioContext scenarioContext, IScenarioStepContext stepContext, Exception hookException)
        {
            HookType = hookType;
            HookException = hookException;
            FeatureContext = featureContext;
            ScenarioContext = scenarioContext;
            StepContext = stepContext;
        }
    }

    public class ScenarioSkippedEvent : ExecutionEvent
    { }

    public class StepSkippedEvent : ExecutionEvent
    { }

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

    public interface IExecutionOutputEvent
    { }

    public class OutputAddedEvent : ExecutionEvent, IExecutionOutputEvent
    {
        public string Text { get; }

        public OutputAddedEvent(string text)
        {
            Text = text;
        }
    }

    public class AttachmentAddedEvent : ExecutionEvent, IExecutionOutputEvent
    {
        public string FilePath { get; }

        public AttachmentAddedEvent(string filePath)
        {
            FilePath = filePath;
        }
    }
}
