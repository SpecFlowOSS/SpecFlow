using BoDi;

namespace TechTalk.SpecFlow.Infrastructure
{
    public interface IContainerDependentObject
    {
        void SetObjectContainer(IObjectContainer container);
    }
}
