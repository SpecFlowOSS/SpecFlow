using System;

namespace SpecFlow.Windsor
{
    [Flags]
    public enum SpecFlowDependencies
    {
        None = 0,
        Contexts = 1,
        Bindings = 2,
        All = Contexts | Bindings
    }
}
