using FluentAssertions;
using TechTalk.SpecFlow.Plugins;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.Infrastructure
{
    public class RuntimePluginLocationMergerTests
    {
        [Fact]
        public void Merge_EmptyList_EmptyList()
        {
            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { });


            //ASSERT
            result.Should().HaveCount(0);
        }

        [Fact]
        public void Merge_SingleEntry_ThisIsReturned()
        {
            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[]{ "C:\\temp\\Plugin.SpecFlowPlugin.dll" } );


            //ASSERT
            result.Should().Contain("C:\\temp\\Plugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Merge_SamePluginDifferentPath_FirstEntryIsReturned()
        {
            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { "C:\\temp\\Plugin.SpecFlowPlugin.dll", "C:\\anotherFolder\\Plugin.SpecFlowPlugin.dll" });


            //ASSERT
            result.Should().Contain("C:\\temp\\Plugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(1);
        }

        [Fact]
        public void Merge_DifferendPluginSamePath_BothAreReturned()
        {
            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { "C:\\temp\\Plugin.SpecFlowPlugin.dll", "C:\\temp\\AnotherPlugin.SpecFlowPlugin.dll" });


            //ASSERT
            result.Should().Contain("C:\\temp\\Plugin.SpecFlowPlugin.dll");
            result.Should().Contain("C:\\temp\\AnotherPlugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(2);
        }
    }
}