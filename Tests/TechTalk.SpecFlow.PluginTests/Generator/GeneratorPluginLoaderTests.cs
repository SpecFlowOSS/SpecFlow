using System;
using System.IO;
using System.Reflection;
using FluentAssertions;
using SpecFlow.Tools.MsBuild.Generation.FrameworkDependent;
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

#if !NETFRAMEWORK
        [Fact]
        public void LoadPlugin_LoadXUnit_WithCustomAssemblyLoader_Successfully()
        {
            //ARRANGE
            string taskAssemblyPath = new Uri(typeof(GeneratorPluginLoader).Assembly.CodeBase).LocalPath;

            var customAssemblyLoader = new AssemblyLoadContextBuilder()
                .SetBaseDirectory(Path.GetDirectoryName(taskAssemblyPath))
                .Build();

            var loadFromAssemblyPath = customAssemblyLoader.LoadFromAssemblyPath(taskAssemblyPath);
            Type innerTaskType = loadFromAssemblyPath.GetType(typeof(GeneratorPluginLoader).FullName);
            

            object innerTask = Activator.CreateInstance(innerTaskType);


            //ACT
            var pluginDescriptor = new PluginDescriptor("TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin", "TechTalk.SpecFlow.xUnit.Generator.SpecFlowPlugin.dll", PluginType.Generator, "");
         
            //var generatorPlugin = innerTask.LoadPlugin(pluginDescriptor);
            var executeInnerMethod = innerTaskType.GetMethod("LoadPlugin", BindingFlags.Instance | BindingFlags.Public);
            var parameters = new object[] { pluginDescriptor };
            var generatorPlugin = executeInnerMethod.Invoke(innerTask, parameters);


            //ASSERT
            generatorPlugin.Should().BeOfType<GeneratorPlugin>();
        }
#endif
    }
}