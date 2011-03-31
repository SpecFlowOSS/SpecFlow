using System;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.Generator.Configuration
{
    public class RuntimeSpecFlowProjectConfigurationLoader : SpecFlowProjectConfigurationLoader
    {
        protected override Version GetGeneratorVersion(IProjectReference projectReference)
        {
            return TestGeneratorFactory.GeneratorVersion;
        }
    }
}