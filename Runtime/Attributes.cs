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
    public abstract class HookAttribute : Attribute
    {
        internal BindingEvent Event { get; private set; }
        public string[] Tags { get; private set; }

        internal HookAttribute(BindingEvent bindingEvent, string[] tags)
        {
            Event = bindingEvent;
            Tags = tags;
        }
    }

    public class BeforeTestRunAttribute : HookAttribute
    {
        public BeforeTestRunAttribute() : base(BindingEvent.TestRunStart, null) {}
    }

    public class AfterTestRunAttribute : HookAttribute
    {
        public AfterTestRunAttribute() : base(BindingEvent.TestRunEnd, null) { }
    }

    public class BeforeFeatureAttribute : HookAttribute
    {
        public BeforeFeatureAttribute(params string[] tags) : base(BindingEvent.FeatureStart, tags) { }
    }

    public class AfterFeatureAttribute : HookAttribute
    {
        public AfterFeatureAttribute(params string[] tags) : base(BindingEvent.FeatureEnd, tags) { }
    }

    /// <summary>
    /// Specifies a hook to be executed before each scenario.
    /// </summary>
    public class BeforeScenarioAttribute : HookAttribute
    {
        public BeforeScenarioAttribute(params string[] tags) : base(BindingEvent.ScenarioStart, tags) { }
    }

    /// <summary>
    /// Specifies a hook to be executed before each scenario. This attribute is a synonym to <see cref="BeforeScenarioAttribute"/>.
    /// </summary>
    public class BeforeAttribute : BeforeScenarioAttribute
    {
        public BeforeAttribute(params string[] tags) : base(tags) { }
    }

    /// <summary>
    /// Specifies a hook to be executed after each scenario.
    /// </summary>
    public class AfterScenarioAttribute : HookAttribute
    {
        public AfterScenarioAttribute(params string[] tags) : base(BindingEvent.ScenarioEnd, tags) { }
    }

    /// <summary>
    /// Specifies a hook to be executed after each scenario. This attribute is a synonym to <see cref="AfterScenarioAttribute"/>.
    /// </summary>
    public class AfterAttribute : AfterScenarioAttribute
    {
        public AfterAttribute(params string[] tags) : base(tags) { }
    }

    public class BeforeScenarioBlockAttribute : HookAttribute
    {
        public BeforeScenarioBlockAttribute(params string[] tags) : base(BindingEvent.BlockStart, tags) { }
    }

    public class AfterScenarioBlockAttribute : HookAttribute
    {
        public AfterScenarioBlockAttribute(params string[] tags) : base(BindingEvent.BlockEnd, tags) { }
    }

    public class BeforeStepAttribute : HookAttribute
    {
        public BeforeStepAttribute(params string[] tags) : base(BindingEvent.StepStart, tags) { }
    }

    public class AfterStepAttribute : HookAttribute
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

    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class StepScopeAttribute : Attribute
    {
        public string Tag { get; set; }
        public string Feature { get; set; }
        public string Scenario { get; set; }
    }
}