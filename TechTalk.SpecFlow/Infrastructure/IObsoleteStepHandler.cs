using TechTalk.SpecFlow.Bindings;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IObsoleteStepHandler
    {
        void Handle(BindingMatch bindingMatch);
    }
}