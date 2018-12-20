using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin;
using Xunit;

namespace TechTalk.SpecFlow.PluginTests.Generator
{
    public class GeneratorPluginLoaderTests
    {
        [Fact]
        public void LoadPlugin_LoadXUnitSuccessfully()
        {
            //ARRANGE
            var generatorPluginLoader = new GeneratorPluginLoader();

            //ACT
            var pluginDescriptor = new PluginDescriptor("TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin", "TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin.dll", PluginType.Generator, "");
            var generatorPlugin = generatorPluginLoader.LoadPlugin(pluginDescriptor);

            //ASSERT
            generatorPlugin.Should().BeOfType<GeneratorPlugin>();
        }

    }
}