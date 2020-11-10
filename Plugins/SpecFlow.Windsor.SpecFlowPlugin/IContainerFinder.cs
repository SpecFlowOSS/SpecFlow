using System;
using Castle.Windsor;

namespace SpecFlow.Windsor
{
    public interface IContainerFinder
    {
        Func<IWindsorContainer> GetCreateScenarioContainer();
    }
}
