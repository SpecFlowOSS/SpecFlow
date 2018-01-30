using System;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator.Helpers;
using TechTalk.SpecFlow.Generator.Project;

namespace TechTalk.SpecFlow.GeneratorTests
{
    [TestFixture]
    public class MSBuildRelativePathParserTests
    {
        private string _directoryName;

        [SetUp]
        public void Setup()
        {
            _directoryName = Path.Combine(Path.GetDirectoryName(new Uri(GetType().Assembly.CodeBase).LocalPath), "Data");
        }

        [Test]
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

        [Test]
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

        [Test]
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

        [Test]
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