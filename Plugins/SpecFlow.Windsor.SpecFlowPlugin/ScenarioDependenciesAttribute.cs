using System;

namespace SpecFlow.Windsor
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioDependenciesAttribute : Attribute
    {
        public SpecFlowDependencies AutoRegister { get; set; } = SpecFlowDependencies.All;
    }
}
