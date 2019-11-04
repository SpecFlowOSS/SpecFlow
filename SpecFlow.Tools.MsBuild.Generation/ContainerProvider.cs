using System.Collections.Generic;
using BoDi;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace SpecFlow.Tools.MsBuild.Generation
{
    public static class ContainerProvider
    {
        public static IObjectContainer GetContainer(ProjectSettings projectSettings, IEnumerable<string> generatorPlugins)
        {
            var container = GeneratorContainerBuilder.CreateContainer(projectSettings.ConfigurationHolder, projectSettings, generatorPlugins);
            return container;
        }
    }
}
