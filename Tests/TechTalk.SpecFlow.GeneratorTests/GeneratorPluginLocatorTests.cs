using System;
using System.Collections;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class GeneratorPluginLocatorTests
    {
        public static IEnumerable BuiltInPluginTestCases
        {
            get
            {
                var version = typeof(GeneratorPluginLoader).Assembly.GetName().Version;
                yield return new TestCaseData($@"\tools", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools\SpecFlowPlugin", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools\SpecFlowPlugin", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\tools", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib\net45", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib\net45", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib\net40", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib\net40", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib\net35", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib\net35", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib", @"SpecFlowPlugin");
                yield return new TestCaseData($@"\tools\lib", @"Generator.SpecFlowPlugin");
            }
        }

        public static IEnumerable ThirdPartPluginTestCases
        {
            get
            {
                var version = typeof(GeneratorPluginLoader).Assembly.GetName().Version;
                yield return new TestCaseData(@"specflow", $@"", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools\SpecFlowPlugin", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools\SpecFlowPlugin", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\tools", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib\net45", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib\net45", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib\net40", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib\net40", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib\net35", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib\net35", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflow", $@"\lib", @"Generator.SpecFlowPlugin");

                yield return new TestCaseData(@"specflowplugin", $@"", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}-{version.Revision}", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools\SpecFlowPlugin.{version.Major}-{version.Minor}", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools\SpecFlowPlugin", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools\SpecFlowPlugin", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\tools", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib\net45", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib\net45", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib\net40", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib\net40", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib\net35", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib\net35", @"Generator.SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib", @"SpecFlowPlugin");
                yield return new TestCaseData(@"specflowplugin", $@"\lib", @"Generator.SpecFlowPlugin");
            }
        }

        [Test, TestCaseSource(nameof(BuiltInPluginTestCases))]
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

        [Test, TestCaseSource(nameof(ThirdPartPluginTestCases))]
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

        [Test, TestCaseSource(nameof(ThirdPartPluginTestCases))]
        public void LocatePluginAssembly_ShouldResolveAssembly_GlobalPackagesDirectory(String packageSuffix, String relativeAssemblyPath, String assemblySuffix)
        {
            // Arrange
            var projectSettings = new ProjectSettings
            {
                ProjectFolder = @"C:\Projects\Project\Project.Tests"
            };

            var fileSystem = new VirtualFileSystem();
            fileSystem.AddFiles($@"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools\TechTalk.SpecFlow.Generator.dll");
            fileSystem.AddFiles($@"C:\Users\jdoe\.nuget\packages\samplegenerator.{packageSuffix}\1.0.0{relativeAssemblyPath}\SampleGenerator.{assemblySuffix}.dll");

            var loader = new GeneratorPluginLocator(projectSettings, @"C:\Users\jdoe\.nuget\packages\specflow\9.9.9\tools", fileSystem);

            var pluginDescriptor = new PluginDescriptor("SampleGenerator", null, PluginType.Generator, null);

            // Act
            var path = loader.LocatePluginAssembly(pluginDescriptor);

            // Assert
            var seperator = $"{Environment.NewLine}\t";
            path.Should().NotBeNull($"the path should match one of the following paths:{seperator}{String.Join($"{seperator}", fileSystem.ProbedPaths)}");
            path.Should().Be($@"C:\Users\jdoe\.nuget\packages\samplegenerator.{packageSuffix}\1.0.0{relativeAssemblyPath}\SampleGenerator.{assemblySuffix}.dll");
        }

        [TestCase(@"C:\Users\jdoe\.nuget\packages\samplegenerator\1.0.0\lib\", Description = "FullPath")]
        [TestCase(@"%TEST_USERPROFILE%\.nuget\packages\samplegenerator\1.0.0\lib\", Description = "PathWithEnvironmentVariable")]
        public void LocatePluginAssembly_ShouldResolveAssembly_PluginPath(string pluginPath)
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

        [Test]
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
                .ShouldThrow<SpecFlowException>()
                .WithMessage("Unable to find plugin in the plugin search path: SampleGenerator. Please check http://go.specflow.org/doc-plugins for details.");
        }

        [Test]
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
                .ShouldThrow<SpecFlowException>()
                .WithMessage("Unable to find plugin in the plugin search path: SampleGenerator. Please check http://go.specflow.org/doc-plugins for details.");
        }
    }
}
