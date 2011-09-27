using System.Collections.Generic;

namespace TechTalk.SpecFlow.Bindings
{
    public interface IBindingRegistryNew
    {
        bool Ready { get; }
        IEnumerable<StepBindingNew> GetConsideredBindings(string stepText);
    }
}