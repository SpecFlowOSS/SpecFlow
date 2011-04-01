using System;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator
{
    public class TestGeneratorFactory : ITestGeneratorFactory
    {
        static public readonly Version GeneratorVersion = new Version("1.5.0.0");

        public Version GetGeneratorVersion()
        {
            return GeneratorVersion;
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings)
        {
            var container = GeneratorContainerBuilder.CreateContainer(projectSettings.ConfigurationHolder);
            container.RegisterInstanceAs(projectSettings);
            return container.Resolve<ITestGenerator>();
        }
    }
}
