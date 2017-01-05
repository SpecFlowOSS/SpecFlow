using System;

namespace TechTalk.SpecFlow
{
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
}