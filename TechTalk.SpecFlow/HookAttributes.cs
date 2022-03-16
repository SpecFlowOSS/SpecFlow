using System;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class HookAttribute : Attribute
    {
        internal HookType Event { get; private set; }
        public string[] Tags { get; private set; }
        /// <summary>
        /// The order in which the hook will be executed. Lower numbers go first.
        /// Orders are only applicable for hooks of the same type.
        /// Hooks with the same priority will have non-deterministic execution order.
        /// Default value is 10,000.
        /// </summary>
        public int Order { get; set; }
        public const int DefaultOrder = 10000;

        internal HookAttribute(HookType bindingEvent, string[] tags)
        {
            Event = bindingEvent;
            Tags = tags;
            Order = DefaultOrder;
        }
    }


    public class BeforeTestRunAttribute : HookAttribute
    {
        /// <summary>
        /// Constructs a new BeforeTestRunAttribute with a default Order of 10000
        /// </summary>
        public BeforeTestRunAttribute() : base(HookType.BeforeTestRun, null) { }

    }

    public class AfterTestRunAttribute : HookAttribute
    {
        public AfterTestRunAttribute() : base(HookType.AfterTestRun, null) { }
    }

    public class BeforeFeatureAttribute : HookAttribute
    {
        public BeforeFeatureAttribute(params string[] tags) : base(HookType.BeforeFeature, tags) { }
    }

    public class AfterFeatureAttribute : HookAttribute
    {
        public AfterFeatureAttribute(params string[] tags) : base(HookType.AfterFeature, tags) { }
    }

    /// <summary>
    /// Specifies a hook to be executed before each scenario.
    /// </summary>
    public class BeforeScenarioAttribute : HookAttribute
    {
        public BeforeScenarioAttribute(params string[] tags) : base(HookType.BeforeScenario, tags) { }
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
        public AfterScenarioAttribute(params string[] tags) : base(HookType.AfterScenario, tags) { }
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
        public BeforeScenarioBlockAttribute(params string[] tags) : base(HookType.BeforeScenarioBlock, tags) { }
    }

    public class AfterScenarioBlockAttribute : HookAttribute
    {
        public AfterScenarioBlockAttribute(params string[] tags) : base(HookType.AfterScenarioBlock, tags) { }
    }

    public class BeforeStepAttribute : HookAttribute
    {
        public BeforeStepAttribute(params string[] tags) : base(HookType.BeforeStep, tags) { }
    }

    public class AfterStepAttribute : HookAttribute
    {
        public AfterStepAttribute(params string[] tags) : base(HookType.AfterStep, tags) { }
    }
}