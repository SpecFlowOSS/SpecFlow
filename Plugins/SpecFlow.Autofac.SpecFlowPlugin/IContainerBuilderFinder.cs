using System;
using Autofac;

namespace SpecFlow.Autofac
{
    public interface IContainerBuilderFinder
    {
        Func<ContainerBuilder, ContainerBuilder> GetConfigureScenarioContainer();

        Func<ContainerBuilder, ContainerBuilder> GetConfigureGlobalContainer();

        Func<ContainerBuilder, ContainerBuilder> GetCreateScenarioContainerBuilder();

        Func<ILifetimeScope> GetFeatureLifetimeScope();
    }
}