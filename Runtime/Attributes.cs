using System;
using System.Linq;

namespace TechTalk.SpecFlow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BindingAttribute : Attribute
    {

    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class ScenarioStepAttribute : Attribute
    {
        internal BindingType Type { get; private set; }
        public string Regex { get; set; }

        internal ScenarioStepAttribute(BindingType type, string regex)
        {
            Type = type;
            Regex = regex;
        }
    }

    public class GivenAttribute : ScenarioStepAttribute
    {
        public GivenAttribute(string regex)
            : base(BindingType.Given, regex)
        {
        }
    }

    public class WhenAttribute : ScenarioStepAttribute
    {
        public WhenAttribute(string regex)
            : base(BindingType.When, regex)
        {
        }
    }

    public class ThenAttribute : ScenarioStepAttribute
    {
        public ThenAttribute(string regex)
            : base(BindingType.Then, regex)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class BindingEventAttribute : Attribute
    {
        internal BindingEvent Event { get; private set; }
        public string[] Tags { get; private set; }

        internal BindingEventAttribute(BindingEvent bindingEvent, string[] tags)
        {
            Event = bindingEvent;
            Tags = tags;
        }
    }

    public class BeforeTestRun : BindingEventAttribute
    {
        public BeforeTestRun() : base(BindingEvent.TestRunStart, null) {}
    }

    public class AfterTestRun : BindingEventAttribute
    {
        public AfterTestRun() : base(BindingEvent.TestRunEnd, null) { }
    }

    public class BeforeFeature : BindingEventAttribute
    {
        public BeforeFeature(params string[] tags) : base(BindingEvent.FeatureStart, tags) { }
    }

    public class AfterFeature : BindingEventAttribute
    {
        public AfterFeature(params string[] tags) : base(BindingEvent.FeatureEnd, tags) { }
    }

    public class BeforeScenario : BindingEventAttribute
    {
        public BeforeScenario(params string[] tags) : base(BindingEvent.ScenarioStart, tags) { }
    }

    public class AfterScenario : BindingEventAttribute
    {
        public AfterScenario(params string[] tags) : base(BindingEvent.ScenarioEnd, tags) { }
    }

    public class BeforeScenarioBlock : BindingEventAttribute
    {
        public BeforeScenarioBlock(params string[] tags) : base(BindingEvent.BlockStart, tags) { }
    }

    public class AfterScenarioBlock : BindingEventAttribute
    {
        public AfterScenarioBlock(params string[] tags) : base(BindingEvent.BlockEnd, tags) { }
    }

    public class BeforeStep : BindingEventAttribute
    {
        public BeforeStep(params string[] tags) : base(BindingEvent.StepStart, tags) { }
    }

    public class AfterStep : BindingEventAttribute
    {
        public AfterStep(params string[] tags) : base(BindingEvent.StepEnd, tags) { }
    }
}