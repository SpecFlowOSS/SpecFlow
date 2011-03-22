using System.Collections.Generic;

namespace TechTalk.SpecFlow.Vs2010Integration.Bindings
{
    public interface IBindingRegistry
    {
        bool Ready { get; }
        IEnumerable<StepBinding> GetConsideredBindings(string stepText);
    }
}