using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingRegistry
    {
        bool Ready { get; }
        IEnumerable<StepBinding> GetConsideredBindings(string stepText);
    }
}