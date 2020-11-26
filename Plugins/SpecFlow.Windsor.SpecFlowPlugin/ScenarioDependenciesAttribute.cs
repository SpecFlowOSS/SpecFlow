using System;

namespace SpecFlow.Windsor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioDependenciesAttribute : Attribute
    {
        public bool AutoRegisterBindings { get; set; } = true;
    }
}
