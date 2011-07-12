using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingRegistry
    {
        bool Ready { get; }
        IEnumerable<StepBindingNew> GetConsideredBindings(string stepText);
    }
}