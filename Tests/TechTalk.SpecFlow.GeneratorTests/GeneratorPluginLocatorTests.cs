using System;
using System.Collections;
using FluentAssertions;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Plugins;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class GeneratorPluginLocatorTests
    {
        public static System.Collections.Generic.IEnumerable<object[]> BuiltInPluginTestCases
        {
            get
            {
                var version = typeof(GeneratorPluginLoader).Assembly.GetName().Version;
                yield return new object[] {$@"\tools", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools\SpecFlowPlugin", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools\SpecFlowPlugin", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools\tools", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\lib\net45", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools\lib\net45", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\lib\net40", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools\lib\net40", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\lib\net35", @"SpecFlowPlugin"};
                yield return new object[] {$@"\tools\lib\net35", @"Generator.SpecFlowPlugin"};
                yield return new object[] {$@"\tools\lib", @"SpecFlowPlugin"};
                yield return new object[] { $@"\tools\lib", @"Generator.SpecFlowPlugin"};
            }
        }

        public static System.Collections.Generic.IEnumerable<object[]> ThirdPartPluginTestCases
        {
            get
            {
                var version = typeof(GeneratorPluginLoader).Assembly.GetName().Version;
                yield return new object[] {@"specflow", $@"", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools\SpecFlowPlugin", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools\SpecFlowPlugin", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\tools", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib\net45", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib\net45", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib\net40", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib\net40", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib\net35", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib\net35", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib", @"SpecFlowPlugin"};
                yield return new object[] {@"specflow", $@"\lib", @"Generator.SpecFlowPlugin"};
                                 
                yield return new object[] {@"specflowplugin", $@"", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools\SpecFlowPlugin", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools\SpecFlowPlugin", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\tools", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\lib\net45", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\lib\net45", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\lib\net40", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\lib\net40", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\lib\net35", @"SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\lib\net35", @"Generator.SpecFlowPlugin"};
                yield return new object[] {@"specflowplugin", $@"\lib", @"SpecFlowPlugin"};
                yield return new object[] { @"specflowplugin", $@"\lib", @"Generator.SpecFlowPlugin"};
            }
        }

        [Theory]
        [MemberData(nameof(BuiltInPluginTestCases))]
        public void LocatePluginAssembly_ShouldResolveAssembly_SolutionPackagesDirectory_BuiltInPlugin(String relativeAssemblyPath, String assemblySuffix)
        {
            // Arrange
            var projectSettings = new ProjectSettings
            {
                ProjectFolder = @"C:\Projects\Project\Project.Tests"
            };

            var fileSystem = new VirtualFileSystem();
            fileSystem.AddFiles($@"C:\Projects\Project\packages\specflow.9.9.9\tools\TechTalk.SpecFlow.Generator.dll");
            fileSystem.AddFiles($@"C:\Projects\Project\packages\specflow.9.9.9{relativeAssemblyPath}\SampleGenerator.{assemblySuffix}.dll");

            var loader = new GeneratorPluginLocator(projectSettings, @"C:\Projects\Project\packages\specflow.9.9.9\tools", fileSystem);

            var pluginDescriptor = new PluginDescriptor("SampleGenerator", null, PluginType.Generator, null);

            // Act
            var path = loader.LocatePluginAssembly(pluginDescriptor);

            // Assert
            var seperator = $"{Environment.NewLine}\t";
            path.Should().NotBeNull($"the path should match one of the following paths:{seperator}{String.Join($"{seperator}", fileSystem.ProbedPaths)}");
            path.Should().Be($@"C:\Projects\Project\packages\specflow.9.9.9{relativeAssemblyPath}\SampleGenerator.{assemblySuffix}.dll");
        }

        [Theory]
        [MemberData(nameof(ThirdPartPluginTestCases))]
        public void LocatePluginAssembly_ShouldResolveAssembly_SolutionPackagesDirectory_ThirdPartyPlugin(String packageSuffix, String relativeAssemblyPath, String assemblySuffix)
        {
            // Arrange
            var projectSettings = new ProjectSettings
            {
                ProjectFolder = @"C:\Projects\Project\Project.Tests"
            };

            var fileSystem = new VirtualFileSystem();
            fileSystem.AddFiles($@"C:\Projects\Project\packages\specflow.9.9.9\tools\TechTalk.SpecFlow.Generator.dll");
            fileSystem.AddFiles($@"C:\Projects\Project\packages\samplegenerator.{packageSuffix}.1.0.0{relativeAssemblyPath}\SampleGenerator.{assemblySuffix}.dll");

            var loader = new GeneratorPluginLocator(projectSettings, @"C:\Projects\Project\packages\specflow.9.9.9\tools", fileSystem);

            var pluginDescriptor = new PluginDescriptor("SampleGenerator", null, PluginType.Generator, null);

            // Act
            var path = loader.LocatePluginAssembly(pluginDescriptor);

            // Assert
            var seperator = $"{Environment.NewLine}\t";
            path.Should().NotBeNull($"the path should match one of the following paths:{seperator}{String.Join($"{seperator}", fileSystem.ProbedPaths)}");
            path.Should().Be($@"C:\Projects\Project\packages\samplegenerator.{packageSuffix}.1.0.0{relativeAssemblyPath}\SampleGenerator.{assemblySuffix}.dll");
        }


        [Theory]
        [InlineData(@"C:\Users\jdoe\.nuget\packages\samplegenerator\1.0.0\lib\")]
        [InlineData(@"%TEST_USERPROFILE%\.nuget\packages\samplegenerator\1.0.0\lib\")]
        public void LocatePluginAssembly_ShouldResolveAssembly_RootedPluginPath(string pluginPath)
        {
            // Arrange
            var projectSettings = new ProjectSettings
            {
                ProjectFolder = @"C:\Projects\Project\Project.Tests"
            };

            Environment.SetEnvironmentVariable("TEST_USERPROFILE", @"C:\Users\jdoe", EnvironmentVariableTarget.Process);

            var fileSystem = new VirtualFileSystem();
            fileSystem.AddFiles(@"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools\TechTalk.SpecFlow.Generator.dll");
            fileSystem.AddFiles(@"C:\Users\jdoe\.nuget\packages\samplegenerator\1.0.0\lib\SampleGenerator.SpecFlowPlugin.dll");

            var loader = new GeneratorPluginLocator(projectSettings, @"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools", fileSystem);

            var pluginDescriptor = new PluginDescriptor("SampleGenerator", pluginPath, PluginType.Generator, null);

            // Act
            var path = loader.LocatePluginAssembly(pluginDescriptor);

            // Assert
            var seperator = $"{Environment.NewLine}\t";
            path.Should().NotBeNull($"the path should match one of the following paths:{seperator}{String.Join($"{seperator}", fileSystem.ProbedPaths)}");
            path.Should().Be(@"C:\Users\jdoe\.nuget\packages\samplegenerator\1.0.0\lib\SampleGenerator.SpecFlowPlugin.dll");
        }

        [Theory]
        [InlineData(@"..\packages\samplegenerator.1.0.0\lib\")]
        [InlineData(@"..\packages\samplegenerator.%SAMPLEGENERATOR_PLUGIN_VERSION%\lib\")]
        public void LocatePluginAssembly_ShouldResolveAssembly_RelativePluginPath(string pluginPath)
        {
            // Arrange
            var projectSettings = new ProjectSettings
            {
                ProjectFolder = @"C:\Projects\Project\Project.Tests"
            };

            Environment.SetEnvironmentVariable("SAMPLEGENERATOR_PLUGIN_VERSION", @"1.0.0", EnvironmentVariableTarget.Process);

            var fileSystem = new VirtualFileSystem();
            fileSystem.AddFiles(@"C:\Projects\Project\packages\specflow.9.9.9\tools\TechTalk.SpecFlow.Generator.dll");
            fileSystem.AddFiles(@"C:\Projects\Project\packages\samplegenerator.1.0.0\lib\SampleGenerator.SpecFlowPlugin.dll");

            var loader = new GeneratorPluginLocator(projectSettings, @"C:\Projects\Project\packages\specflow.9.9.9\tools", fileSystem);

            var pluginDescriptor = new PluginDescriptor("SampleGenerator", pluginPath, PluginType.Generator, null);

            // Act
            var path = loader.LocatePluginAssembly(pluginDescriptor);

            // Assert
            var seperator = $"{Environment.NewLine}\t";
            path.Should().NotBeNull($"the path should match one of the following paths:{seperator}{String.Join($"{seperator}", fileSystem.ProbedPaths)}");
            path.Should().Be(@"C:\Projects\Project\packages\samplegenerator.1.0.0\lib\SampleGenerator.SpecFlowPlugin.dll");
        }

        [Fact]
        public void LocatePluginAssembly_ShouldThrow_WhenPluginDoesNotExist()
        {
            // Arrange
            var projectSettings = new ProjectSettings
            {
                ProjectFolder = @"C:\Projects\Project\Project.Tests"
            };

            var fileSystem = new VirtualFileSystem();
            fileSystem.AddFiles($@"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools\TechTalk.SpecFlow.Generator.dll");

            var loader = new GeneratorPluginLocator(projectSettings, @"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools", fileSystem);

            var pluginDescriptor = new PluginDescriptor("SampleGenerator", null, PluginType.Generator, null);

            // Act
            Action action = () => loader.LocatePluginAssembly(pluginDescriptor);

            // Assert
            action
                .Should().Throw<SpecFlowException>()
                .WithMessage("Unable to find plugin in the plugin search path: SampleGenerator. Please check http://go.specflow.org/doc-plugins for details.");
        }

        [Fact]
        public void LocatePluginAssembly_ShouldThrow_WhenPluginPathDoesNotExist()
        {
            // Arrange
            var projectSettings = new ProjectSettings
            {
                ProjectFolder = @"C:\Projects\Project\Project.Tests"
            };

            var fileSystem = new VirtualFileSystem();
            fileSystem.AddFiles(@"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools\TechTalk.SpecFlow.Generator.dll");

            var loader = new GeneratorPluginLocator(projectSettings, @"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools", fileSystem);

            var pluginDescriptor = new PluginDescriptor("SampleGenerator", @"C:\Users\jdoe\.nuget\packages\samplegenerator\1.0.0\lib\", PluginType.Generator, null);

            // Act
            Action action = () => loader.LocatePluginAssembly(pluginDescriptor);

            // Assert
            action
                .Should().Throw<SpecFlowException>()
                .WithMessage("Unable to find plugin in the plugin search path: SampleGenerator. Please check http://go.specflow.org/doc-plugins for details.");
        }
    }
}
