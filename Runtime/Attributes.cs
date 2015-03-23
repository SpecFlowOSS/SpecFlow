using System;
using System.Linq;
using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow
{
    /// <summary>
    /// Marker attribute that specifies that this class may contain bindings (step definitions, hooks, etc.)
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class BindingAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class StepDefinitionBaseAttribute : Attribute
    {
        internal StepDefinitionType[] Types { get; private set; }
        public string Regex { get; set; }

        internal StepDefinitionBaseAttribute(string regex, StepDefinitionType type)
            : this(regex, new[] { type })
        {
        }

        protected StepDefinitionBaseAttribute(string regex, StepDefinitionType[] types)
        {
            if (types == null) throw new ArgumentNullException("types");
            if (types.Length == 0) throw new ArgumentException("List cannot be empty", "types");

            Regex = regex;
            Types = types;
        }
    }

    /// <summary>
    /// Specifies a 'Given' step definition that matches for the provided regular expression.
    /// </summary>
    public class GivenAttribute : StepDefinitionBaseAttribute
    {
        public GivenAttribute() : this(null)
        {
        }


        public GivenAttribute(string regex)
            : base(regex, StepDefinitionType.Given)
        {
        }
    }

    /// <summary>
    /// Specifies a 'When' step definition that matches for the provided regular expression.
    /// </summary>
    public class WhenAttribute : StepDefinitionBaseAttribute
    {
        public WhenAttribute() : this(null)
        {
        }

        public WhenAttribute(string regex)
            : base(regex, StepDefinitionType.When)
        {
        }
    }

    /// <summary>
    /// Specifies a 'Then' step definition that matches for the provided regular expression.
    /// </summary>
    public class ThenAttribute : StepDefinitionBaseAttribute
    {
        public ThenAttribute() : this(null)
        {
        }

        public ThenAttribute(string regex)
            : base(regex, StepDefinitionType.Then)
        {
        }
    }

    /// <summary>
    /// Specifies a step definition that matches for the provided regular expression and any step kinds (given, when, then).
    /// </summary>
    public class StepDefinitionAttribute : StepDefinitionBaseAttribute
    {
        public StepDefinitionAttribute() : this(null)
        {
        }

        public StepDefinitionAttribute(string regex) : base(regex, new[] { StepDefinitionType.Given, StepDefinitionType.When, StepDefinitionType.Then })
        {
        }
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public abstract class HookAttribute : Attribute
    {
        internal HookType Event { get; private set; }
        public string[] Tags { get; private set; }
        public int Priority { get; private set; }
        public const int DefaultPriority = 10000;

        internal HookAttribute(HookType bindingEvent, string[] tags, int priority)
        {
            Event = bindingEvent;
            Tags = tags;
            Priority = priority;
        }
    }

    public class BeforeTestRunAttribute : HookAttribute
    {
        /// <summary>
        /// Constructs a new BeforeTestRunAttribute with a default priority of 10000
        /// </summary>
        public BeforeTestRunAttribute() : base(HookType.BeforeTestRun, null, DefaultPriority) {}
        /// <summary>
        /// Constructs a new BeforeTestRunAttribute
        /// </summary>
        /// <param name="priority">The priority with which methods with this attribute should be executed. Lower numbers will be run first</param>
        public BeforeTestRunAttribute(int priority) : base(HookType.BeforeTestRun, null, priority) {}
        
    }

    public class AfterTestRunAttribute : HookAttribute
    {
        public AfterTestRunAttribute() : base(HookType.AfterTestRun, null, DefaultPriority) { }
        public AfterTestRunAttribute(int priority) : base(HookType.AfterTestRun, null, priority) { }
    }

    public class BeforeFeatureAttribute : HookAttribute
    {
        public BeforeFeatureAttribute(int priority, params string[] tags) : base(HookType.BeforeFeature, tags, priority) { }
        public BeforeFeatureAttribute(params string[] tags) : base(HookType.BeforeFeature, tags, DefaultPriority) { }
    }

    public class AfterFeatureAttribute : HookAttribute
    {
        public AfterFeatureAttribute(int priority, params string[] tags) : base(HookType.AfterFeature, tags, priority) { }
        public AfterFeatureAttribute(params string[] tags) : base(HookType.AfterFeature, tags, DefaultPriority) { }
    }

    /// <summary>
    /// Specifies a hook to be executed before each scenario.
    /// </summary>
    public class BeforeScenarioAttribute : HookAttribute
    {
        public BeforeScenarioAttribute(int priority, params string[] tags) : base(HookType.BeforeScenario, tags, priority) { }
        public BeforeScenarioAttribute(params string[] tags) : base(HookType.BeforeScenario, tags, DefaultPriority) { }
    }

    /// <summary>
    /// Specifies a hook to be executed before each scenario. This attribute is a synonym to <see cref="BeforeScenarioAttribute"/>.
    /// </summary>
    public class BeforeAttribute : BeforeScenarioAttribute
    {
        public BeforeAttribute(int priority, params string[] tags) : base(priority, tags) { }
        public BeforeAttribute(params string[] tags) : base(tags) { }
    }

    /// <summary>
    /// Specifies a hook to be executed after each scenario.
    /// </summary>
    public class AfterScenarioAttribute : HookAttribute
    {
        public AfterScenarioAttribute(int priority,params string[] tags) : base(HookType.AfterScenario, tags, priority) { }
        public AfterScenarioAttribute(params string[] tags) : base(HookType.AfterScenario, tags, DefaultPriority) { }
    }

    /// <summary>
    /// Specifies a hook to be executed after each scenario. This attribute is a synonym to <see cref="AfterScenarioAttribute"/>.
    /// </summary>
    public class AfterAttribute : AfterScenarioAttribute
    {
        public AfterAttribute(int priority, params string[] tags) : base(priority, tags) { }
        public AfterAttribute(params string[] tags) : base(tags) { }
    }

    public class BeforeScenarioBlockAttribute : HookAttribute
    {
        public BeforeScenarioBlockAttribute(int priority,params string[] tags) : base(HookType.BeforeScenarioBlock, tags,priority) { }
        public BeforeScenarioBlockAttribute(params string[] tags) : base(HookType.BeforeScenarioBlock, tags,DefaultPriority) { }
    }

    public class AfterScenarioBlockAttribute : HookAttribute
    {
        public AfterScenarioBlockAttribute(int priority,params string[] tags) : base(HookType.AfterScenarioBlock, tags,priority) { }
        public AfterScenarioBlockAttribute(params string[] tags) : base(HookType.AfterScenarioBlock, tags,DefaultPriority) { }
    }

    public class BeforeStepAttribute : HookAttribute
    {
        public BeforeStepAttribute(int priority=DefaultPriority,params string[] tags) : base(HookType.BeforeStep, tags, priority) { }
        public BeforeStepAttribute(params string[] tags) : base(HookType.BeforeStep, tags, DefaultPriority) { }
    }

    public class AfterStepAttribute : HookAttribute
    {
        public AfterStepAttribute(int priority,params string[] tags) : base(HookType.AfterStep, tags,priority) { }
        public AfterStepAttribute(params string[] tags) : base(HookType.AfterStep, tags,DefaultPriority) { }
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

    /// <summary>
    /// Restricts the binding attributes (step definition, hook, etc.) to be applied only in a specific scope.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
    public class ScopeAttribute : Attribute
    {
        public string Tag { get; set; }
        public string Feature { get; set; }
        public string Scenario { get; set; }
    }

    [Obsolete("Use [Scope] attribute instead.")]
    public class StepScopeAttribute : ScopeAttribute
    {
    }
}