using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.Interfaces;
using TechTalk.SpecFlow.Generator.Plugins;
using TechTalk.SpecFlow.Plugins;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class GeneratorPluginLocatorTests
    {
        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlow", "SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlow", "Generator.SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlow", "SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlow", "Generator.SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlowPlugin", "SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlowPlugin", "Generator.SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlowPlugin", "SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlowPlugin", "Generator.SpecFlowPlugin")]
        public void Should_locate_latest_generator_when_using_local_nuget(
            int specFlowMajor, int specFlowMinor, int specFlowRevision, string pluginName, int pluginMajor, int pluginMinor, int pluginRevision,
            string frameworkMoniker, string pluginNamePostFix, string pluginAssemblyPostFix)
        {
            // Arrange
            var projectSettings = new ProjectSettings()
            {
                ProjectFolder = @"C:\SolutionFolder\ProjectFolder\"
            };
            
            var fileSystem = this.GetMockFileSystem(pluginName, pluginNamePostFix, pluginAssemblyPostFix);

            var executingAssemblyInfoMock = new Mock<IExecutingAssemblyInfo>();

            executingAssemblyInfoMock
                .Setup(x => x.GetCodeBase())
                .Returns(String.Format(@"file:///C:/SolutionFolder/packages/SpecFlow.{0}.{1}.{2}/tools/TechTalk.SpecFlow.Generator.dll", specFlowMajor, specFlowMinor, specFlowRevision));

            executingAssemblyInfoMock
                .Setup(x => x.GetVersion())
                .Returns(new Version(specFlowMajor, specFlowMinor, 0, specFlowRevision));

            var loader = new GeneratorPluginLocator(projectSettings, fileSystem, executingAssemblyInfoMock.Object);

            // Act
            var pluginDescriptor = new PluginDescriptor(pluginName, null, PluginType.GeneratorAndRuntime, null);
            var actual = loader.GetGeneratorPluginAssemblies(pluginDescriptor).Distinct().ToList();

            // Assert
            var expected = new List<string>
            {
                String.Format(@"C:\SolutionFolder\packages\{7}.{8}.{0}-{1}-{2}.{3}.{4}.{5}\lib\{6}\{7}.{9}.dll",
                    specFlowMajor, specFlowMinor, specFlowRevision,
                    pluginMajor, pluginMinor, pluginRevision,
                    frameworkMoniker, pluginName, pluginNamePostFix, pluginAssemblyPostFix)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlow", "SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlow", "Generator.SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlow", "SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlow", "Generator.SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlowPlugin", "SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", 1, 5, 2, "net35", "SpecFlowPlugin", "Generator.SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlowPlugin", "SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", 1, 6, 0, "net45", "SpecFlowPlugin", "Generator.SpecFlowPlugin")]
        public void Should_locate_latest_generator_when_using_global_nuget(
            int specFlowMajor, int specFlowMinor, int specFlowRevision, string pluginName, int pluginMajor, int pluginMinor, int pluginRevision,
            string frameworkMoniker, string pluginNamePostFix, string pluginAssemblyPostFix)
        {
            // Arrange
            var projectSettings = new ProjectSettings()
            {
                ProjectFolder = @"C:\SolutionFolder\ProjectFolder\"
            };
            
            var fileSystem = this.GetMockFileSystem(pluginName, pluginNamePostFix, pluginAssemblyPostFix);

            var executingAssemblyInfoMock = new Mock<IExecutingAssemblyInfo>();

            executingAssemblyInfoMock
                .Setup(x => x.GetCodeBase())
                .Returns(String.Format(@"file:///C:/Users/JDoe/.nuget/packages/SpecFlow/{0}.{1}.{2}/tools/TechTalk.SpecFlow.Generator.dll", specFlowMajor, specFlowMinor, specFlowRevision));

            executingAssemblyInfoMock
                .Setup(x => x.GetVersion())
                .Returns(new Version(specFlowMajor, specFlowMinor, 0, specFlowRevision));

            var loader = new GeneratorPluginLocator(projectSettings, fileSystem, executingAssemblyInfoMock.Object);

            // Act
            var pluginDescriptor = new PluginDescriptor(pluginName, null, PluginType.GeneratorAndRuntime, null);
            var actual = loader.GetGeneratorPluginAssemblies(pluginDescriptor).Distinct().ToList();

            // Assert
            var expected = new List<string>
            {
                String.Format(@"C:\Users\JDoe\.nuget\packages\{7}.{8}.{0}-{1}-{2}\{3}.{4}.{5}\lib\{6}\{7}.{9}.dll",
                    specFlowMajor, specFlowMinor, specFlowRevision,
                    pluginMajor, pluginMinor, pluginRevision,
                    frameworkMoniker, pluginName, pluginNamePostFix, pluginAssemblyPostFix)
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }
        
        [TestCase(1, 9, 0, "SpecRun", @".\bin\Debug\", "SpecFlow", "SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", @".\bin\Debug\", "SpecFlow", "Generator.SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", @".\bin\Debug\", "SpecFlow", "SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", @".\bin\Debug\", "SpecFlow", "Generator.SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", @".\bin\Debug\", "SpecFlowPlugin", "SpecFlowPlugin")]
        [TestCase(1, 9, 0, "SpecRun", @".\bin\Debug\", "SpecFlowPlugin", "Generator.SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", @".\bin\Debug\", "SpecFlowPlugin", "SpecFlowPlugin")]
        [TestCase(2, 1, 0, "SpecRun", @".\bin\Debug\", "SpecFlowPlugin", "Generator.SpecFlowPlugin")]
        public void Should_locate_latest_generator_when_using_path(int specFlowMajor, int specFlowMinor, int specFlowRevision, string pluginName, string path, string pluginNamePostFix, string pluginAssemblyPostFix)
        {
            // Arrange
            var projectSettings = new ProjectSettings()
            {
                ProjectFolder = @"C:\SolutionFolder\ProjectFolder\"
            };

            var fileSystem = this.GetMockFileSystem(pluginName, pluginNamePostFix, pluginAssemblyPostFix);

            var executingAssemblyInfoMock = new Mock<IExecutingAssemblyInfo>();

            executingAssemblyInfoMock
                .Setup(x => x.GetCodeBase())
                .Returns(String.Format(@"file:///C:/SolutionFolder/packages/SpecFlow.{0}.{1}.{2}/tools/TechTalk.SpecFlow.Generator.dll", specFlowMajor, specFlowMinor, specFlowRevision));

            executingAssemblyInfoMock
                .Setup(x => x.GetVersion())
                .Returns(new Version(specFlowMajor, specFlowMinor, 0, specFlowRevision));

            var loader = new GeneratorPluginLocator(projectSettings, fileSystem, executingAssemblyInfoMock.Object);

            // Act
            var pluginDescriptor = new PluginDescriptor(pluginName, path, PluginType.GeneratorAndRuntime, null);
            var actual = loader.GetGeneratorPluginAssemblies(pluginDescriptor).Distinct().ToList();

            // Assert
            var expected = new List<string>
            {
                Path.GetFullPath(Path.Combine(@"C:\SolutionFolder\ProjectFolder\", path, String.Format("{0}.{1}.dll", pluginName, pluginAssemblyPostFix)))
            };

            CollectionAssert.AreEquivalent(expected, actual);
        }

        private IFileSystem GetMockFileSystem(string pluginName, string pluginNamePostFix, string pluginAssemblyPostFix)
        {
            return new MockFileSystem(
                String.Format(@"C:\Users\JDoe\.nuget\packages\{0}.{1}\1.5.2\lib\net45\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\Users\JDoe\.nuget\packages\{0}.{1}.1-9-0\1.5.2\lib\net35\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\Users\JDoe\.nuget\packages\{0}.{1}.2-1-0\1.6.0\lib\net45\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\Users\JDoe\.nuget\packages\{0}.{1}.2-2-0\1.6.0\lib\net45\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\SolutionFolder\packages\{0}.{1}.1.5.2\lib\net45\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\SolutionFolder\packages\{0}.{1}.1-9-0.1.5.2\lib\net35\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\SolutionFolder\packages\{0}.{1}.2-1-0.1.6.0\lib\net45\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\SolutionFolder\packages\{0}.{1}.2-2-0.1.6.0\lib\net45\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix),
                String.Format(@"C:\SolutionFolder\ProjectFolder\bin\Debug\{0}.{2}.dll", pluginName, pluginNamePostFix, pluginAssemblyPostFix));
        }
    }
}
