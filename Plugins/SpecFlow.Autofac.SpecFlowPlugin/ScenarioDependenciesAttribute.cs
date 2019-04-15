using System;
using System.Linq;

namespace SpecFlow.Autofac
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ScenarioDependenciesAttribute : Attribute
    {
    }
}
