using System;
using System.Linq;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BindingAttribute : Attribute
    {
//        private readonly string featureName;
//
//        public BindingAttribute()
//        {
//        }
//
//        public BindingAttribute(string featureName)
//            : this()
//        {
//            this.featureName = featureName;
//        }
//
//        public string FeatureName
//        {
//            get { return featureName; }
//        }
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

    public class BeforeTestRunAttribute : BindingEventAttribute
    {
        public BeforeTestRunAttribute() : base(BindingEvent.TestRunStart, null) {}
    }

    public class AfterTestRunAttribute : BindingEventAttribute
    {
        public AfterTestRunAttribute() : base(BindingEvent.TestRunEnd, null) { }
    }

    public class BeforeFeatureAttribute : BindingEventAttribute
    {
        public BeforeFeatureAttribute(params string[] tags) : base(BindingEvent.FeatureStart, tags) { }
    }

    public class AfterFeatureAttribute : BindingEventAttribute
    {
        public AfterFeatureAttribute(params string[] tags) : base(BindingEvent.FeatureEnd, tags) { }
    }

    public class BeforeScenarioAttribute : BindingEventAttribute
    {
        public BeforeScenarioAttribute(params string[] tags) : base(BindingEvent.ScenarioStart, tags) { }
    }

    public class AfterScenarioAttribute : BindingEventAttribute
    {
        public AfterScenarioAttribute(params string[] tags) : base(BindingEvent.ScenarioEnd, tags) { }
    }

    public class BeforeScenarioBlockAttribute : BindingEventAttribute
    {
        public BeforeScenarioBlockAttribute(params string[] tags) : base(BindingEvent.BlockStart, tags) { }
    }

    public class AfterScenarioBlockAttribute : BindingEventAttribute
    {
        public AfterScenarioBlockAttribute(params string[] tags) : base(BindingEvent.BlockEnd, tags) { }
    }

    public class BeforeStepAttribute : BindingEventAttribute
    {
        public BeforeStepAttribute(params string[] tags) : base(BindingEvent.StepStart, tags) { }
    }

    public class AfterStepAttribute : BindingEventAttribute
    {
        public AfterStepAttribute(params string[] tags) : base(BindingEvent.StepEnd, tags) { }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StepArgumentTransformationAttribute : Attribute
    {
        public string Regex { get; set; }

        public StepArgumentTransformationAttribute(string regex)
        {
            Regex = regex;
        }   
        
        public StepArgumentTransformationAttribute()
        {
            Regex = null;
        }
    }

    [Obsolete("this attribute has been renamed to [StepArgumentTransformation]")]
    public class StepTransformationAttribute : StepArgumentTransformationAttribute
    {
        public StepTransformationAttribute(string regex) : base(regex)
        {
        }

        public StepTransformationAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class StepScopeAttribute : Attribute
    {
        public string Tag { get; set; }
        public string Feature { get; set; }
        public string Scenario { get; set; }
    }
}