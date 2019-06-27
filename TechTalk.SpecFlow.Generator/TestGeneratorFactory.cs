using System;
using System.Collections.Generic;
using System.Linq;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.Generator
{
    public class TestGeneratorFactory : RemotableGeneratorClass, ITestGeneratorFactory
    {
        // update this version to the latest version number, if there are changes in the test generation
        public static readonly Version GeneratorVersion = new Version("3.1.0.0");

        public Version GetGeneratorVersion()
        {
            return GeneratorVersion;
        }

        public ITestGenerator CreateGenerator(ProjectSettings projectSettings, IEnumerable<string> generatorPlugins)
        {
            var container = GeneratorContainerBuilder.CreateContainer(projectSettings.ConfigurationHolder, projectSettings, generatorPlugins);
            return container.Resolve<ITestGenerator>();
        }
    }
}
