using System;

namespace TechTalk.SpecFlow
{
    /// <summary>
    /// Specifies the method to be used as a custom step definition parameter conversion.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class StepArgumentTransformationAttribute : Attribute
    {
        /// <summary>
        /// The regular expression that have to match the step argument. The entire argument is passed to the method if omitted.
        /// </summary>
        public string Regex { get; set; }
        /// <summary>
        /// The custom parameter type name to be used in Cucumber Expressions
        /// </summary>
        public string Name { get; set; }

        public StepArgumentTransformationAttribute(string regex)
        {
            Regex = regex;
        }

        public StepArgumentTransformationAttribute()
        {
            Regex = null;
        }
    }
}