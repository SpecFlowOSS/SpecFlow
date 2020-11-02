﻿using System;
using FluentAssertions;
using TechTalk.SpecFlow.Plugins;
using Xunit;

namespace TechTalk.SpecFlow.PluginTests
{
    public class RuntimePluginLocatorTests
    {
        [Fact]
        public void LoadPlugins_Find_All_5_Referenced_RuntimePlugins()
        {
            //ARRANGE
            var runtimePluginLocator = new RuntimePluginLocator(new RuntimePluginLocationMerger(), new SpecFlowPath());

            //ACT
            var plugins = runtimePluginLocator.GetAllRuntimePlugins();

            //ASSERT
            plugins.Count.Should().Be(5, $"{string.Join(",", plugins)} were found");
        }

    }
}
