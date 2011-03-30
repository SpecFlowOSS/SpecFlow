using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    public interface ITestGeneratorFactory
    {
        Version GetGeneratorVersion();
        ITestGenerator CreateGenerator(SpecFlowConfigurationHolder configurationHolder);
    }
}