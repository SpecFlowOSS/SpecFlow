using System;
using System.Runtime.InteropServices;
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

        [SkippableFact]
        public void Merge_SingleEntry_ThisIsReturned_Windows()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[]{ "C:\\temp\\Plugin.SpecFlowPlugin.dll" } );


            //ASSERT
            result.Should().Contain("C:\\temp\\Plugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(1);
        }

        [SkippableFact]
        public void Merge_SingleEntry_ThisIsReturned_Unix()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||RuntimeInformation.IsOSPlatform(OSPlatform.OSX));

            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { "/temp/Plugin.SpecFlowPlugin.dll" });


            //ASSERT
            result.Should().Contain("/temp/Plugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(1);
        }


        [SkippableFact]
        public void Merge_SamePluginDifferentPath_FirstEntryIsReturned_Windows()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { "C:\\temp\\Plugin.SpecFlowPlugin.dll", "C:\\anotherFolder\\Plugin.SpecFlowPlugin.dll" });

            //ASSERT
            result.Should().Contain("C:\\temp\\Plugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(1);
        }

        [SkippableFact]
        public void Merge_SamePluginDifferentPath_FirstEntryIsReturned_Unix()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX));

            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { "/temp/Plugin.SpecFlowPlugin.dll", "/anotherFolder/Plugin.SpecFlowPlugin.dll" });

            //ASSERT
            result.Should().Contain("/temp/Plugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(1);
        }

        [SkippableFact]
        public void Merge_DifferendPluginSamePath_BothAreReturned_Windows()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { "C:\\temp\\Plugin.SpecFlowPlugin.dll", "C:\\temp\\AnotherPlugin.SpecFlowPlugin.dll" });


            //ASSERT
            result.Should().Contain("C:\\temp\\Plugin.SpecFlowPlugin.dll");
            result.Should().Contain("C:\\temp\\AnotherPlugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(2);
        }


        [SkippableFact]
        public void Merge_DifferendPluginSamePath_BothAreReturned_Unix()
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX));

            //ARRANGE
            var runtimePluginLocationMerger = new RuntimePluginLocationMerger();


            //ACT
            var result = runtimePluginLocationMerger.Merge(new string[] { "/temp/Plugin.SpecFlowPlugin.dll", "/temp/AnotherPlugin.SpecFlowPlugin.dll" });


            //ASSERT
            result.Should().Contain("/temp/Plugin.SpecFlowPlugin.dll");
            result.Should().Contain("/temp/AnotherPlugin.SpecFlowPlugin.dll");
            result.Should().HaveCount(2);
        }
    }
}