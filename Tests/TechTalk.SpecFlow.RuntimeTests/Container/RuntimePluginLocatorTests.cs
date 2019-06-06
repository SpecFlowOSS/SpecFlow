using System;
using FluentAssertions;
using TechTalk.SpecFlow.Plugins;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Container
{
    public class RuntimePluginLocatorTests
    {
        [Fact]
        public void LoadPlugins_Doesnt_load_too_much_assemblies()
        {
            //ARRANGE
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            var runtimePluginLocator = new RuntimePluginLocator(new RuntimePluginLocationMerger(), new SpecFlowPath());


            //ACT
            runtimePluginLocator.GetAllRuntimePlugins();


            //ASSERT
            var nowLoadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

            nowLoadedAssemblies.Should().BeEquivalentTo(loadedAssemblies);
        }
    }
}
