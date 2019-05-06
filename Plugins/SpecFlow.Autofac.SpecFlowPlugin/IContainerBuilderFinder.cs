using System;
using Autofac;

namespace SpecFlow.Autofac
{
    public interface IContainerBuilderFinder
    {
        Func<ContainerBuilder> GetCreateScenarioContainerBuilder();
    }
}