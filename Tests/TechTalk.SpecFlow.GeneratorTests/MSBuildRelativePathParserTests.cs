using System;
using System.IO;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Generator.Helpers;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.GeneratorTests
{
    
    public class MSBuildRelativePathParserTests
    {
        private string _directoryName;

        public MSBuildRelativePathParserTests()
        {
#if NET461
            string assemblyLocation = GetType().Assembly.CodeBase;
#else
            string assemblyLocation = GetType().Assembly.Location;
#endif
            string assemblyFolder = Path.GetDirectoryName(new Uri(assemblyLocation).LocalPath);
            _directoryName = Path.Combine(assemblyFolder, "Data");
        }

        [Fact]
        public void GetFiles_PathWithoutWildcards_ReturnsPath()
        {
            //ARRANGE
            var msBuildRelativePathParser = new MSBuildRelativePathParser();

            //ACT
            var files = msBuildRelativePathParser.GetFiles(_directoryName, PathHelper.SanitizeDirectorySeparatorChar(@"ExampleFeatures\Features\Subfolder1\ExternalFeature1.feature"));

            //ASSERT
            files.Should().Contain(PathHelper.SanitizeDirectorySeparatorChar(@"ExampleFeatures\Features\Subfolder1\ExternalFeature1.feature"));
            files.Count.Should().Be(1);
        }

        [Fact]
        public void GetFiles_PathWithFileWildcards_ReturnsPath()
        {
            //ARRANGE
            var msBuildRelativePathParser = new MSBuildRelativePathParser();

            //ACT
            var files = msBuildRelativePathParser.GetFiles(_directoryName, PathHelper.SanitizeDirectorySeparatorChar(@"ExampleFeatures\Features\Subfolder1\*.feature"));

            //ASSERT
            files.Should().Contain(PathHelper.SanitizeDirectorySeparatorChar(@"ExampleFeatures\Features\Subfolder1\ExternalFeature1.feature"));
            files.Count.Should().Be(1);
        }

        [Fact]
        public void GetFiles_PathWithPathWildcards_ReturnsPath()
        {
            //ARRANGE
            var msBuildRelativePathParser = new MSBuildRelativePathParser();

            //ACT
            var files = msBuildRelativePathParser.GetFiles(_directoryName, PathHelper.SanitizeDirectorySeparatorChar(@"ExampleFeatures\Features\**\*.feature"));

            //ASSERT
            files.Should().Contain(PathHelper.SanitizeDirectorySeparatorChar(@"ExampleFeatures\Features\Subfolder1\ExternalFeature1.feature"));
            files.Should().Contain(PathHelper.SanitizeDirectorySeparatorChar(@"ExampleFeatures\Features\Subfolder2\ExternalFeature2.feature"));
            files.Count.Should().Be(2);
        }

        [Fact]
        public void GetFiles_RelativePathWithFileWildcards_ReturnsPath()
        {
            //ARRANGE
            var msBuildRelativePathParser = new MSBuildRelativePathParser();

            //ACT
            var files = msBuildRelativePathParser.GetFiles(_directoryName, PathHelper.SanitizeDirectorySeparatorChar(@"..\Data\ExampleFeatures\Features\Subfolder1\*.feature"));

            //ASSERT
            files.Should().Contain(PathHelper.SanitizeDirectorySeparatorChar(@"..\Data\ExampleFeatures\Features\Subfolder1\ExternalFeature1.feature"));
            files.Count.Should().Be(1);
        }
    }
}