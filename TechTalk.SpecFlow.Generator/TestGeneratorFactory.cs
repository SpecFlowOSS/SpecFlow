using System;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator
{
    public class TestGeneratorFactory : RemotableGeneratorClass, ITestGeneratorFactory
    {
        // update this version to the latest version number, if there are changes in the test generation
        static public readonly Version GeneratorVersion = new Version("2.2.0.0");

        public Version GetGeneratorVersion()
        {
            return GeneratorVersion;
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings)
        {
            var container = GeneratorContainerBuilder.CreateContainer(projectSettings.ConfigurationHolder, projectSettings);
            return container.Resolve<ITestGenerator>();
        }
    }
}
