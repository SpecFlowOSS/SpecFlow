using System;
using FluentAssertions;
using TechTalk.SpecFlow.Plugins;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Container
{
    public class RuntimePluginLocatorTests
    {
        [Fact]
        public void LoadPlugins_Find_All_3_Referenced_RuntimePlugins()
        {
            //ARRANGE
            var runtimePluginLocator = new RuntimePluginLocator();

            //ACT
            var plugins = runtimePluginLocator.GetAllRuntimePlugins();

            //ASSERT
            plugins.Count.Should().Be(3);
        }

        [Fact]
        public void LoadPlugins_Doesnt_load_too_much_assemblies()
        {
            //ARRANGE
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var runtimePluginLocator = new RuntimePluginLocator();


            //ACT
            runtimePluginLocator.GetAllRuntimePlugins();


            //ASSERT
            var nowLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            nowLoadedAssemblies.Should().BeEquivalentTo(loadedAssemblies);
        }
    }
}
