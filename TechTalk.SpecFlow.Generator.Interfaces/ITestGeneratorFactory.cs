using System;

namespace TechTalk.SpecFlow.Generator
{
    public interface ITestGeneratorFactory
    {
        Version GetGeneratorVersion();
        ITestGenerator CreateGenerator(SpecFlowConfigurationHolder configurationHolder);
    }
}