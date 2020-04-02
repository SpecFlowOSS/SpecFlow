using System;
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
        public StepDefinitionType[] Types { get; private set; }
        public string Regex { get; set; }

        /// <summary>
        /// additional information in which culture the step is written
        /// it does not affect the matching of the step
        /// it is only for tooling support needed
        /// </summary>
        public string Culture { get; set; }

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

        public GivenAttribute(string regex, string culture) 
            : this(regex)
        {
            Culture = culture;
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

        public WhenAttribute(string regex, string culture) 
            : this(regex)
        {
            Culture = culture;
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

        public ThenAttribute(string regex, string culture) 
            : this(regex)
        {
            Culture = culture;
        }
    }

    /// <summary>
    /// Specifies a step definition that matches for the provided regular expression and any step kinds (given, when, then).
    /// </summary>
    public class StepDefinitionAttribute : StepDefinitionBaseAttribute
    {
        public StepDefinitionAttribute() 
            : this(null)
        {
        }

        public StepDefinitionAttribute(string regex) 
            : base(regex, new[] { StepDefinitionType.Given, StepDefinitionType.When, StepDefinitionType.Then })
        {
        }

        public StepDefinitionAttribute(string regex, string culture) 
            : this(regex)
        {
            Culture = culture;
        }
    }
}