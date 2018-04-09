using System.IO;
using FluentAssertions;
using Microsoft.Build.Utilities;
using SpecFlow.Tools.MsBuild.Generation;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class GenerateFeatureFileCodeBehindTaskTests
    {
        [Theory(DisplayName = "FilePathGenerator should generate correct path.")]
        [InlineData(@"C:\temp\Project1", "Features", "Some.feature", "Some.feature.cs", @"C:\temp\Project1\Features\Some.feature.cs")]
        public void FilePathGenerator_GenerateFilePath_ShouldReturnCorrectPath(
            string projectDir,
            string relativeOutputDir,
            string featureFile,
            string generatedCodeBehindName,
            string expected)
        {
            // ARRANGE & ACT
            string actual = FilePathGenerator.GenerateFilePath(projectDir, relativeOutputDir, featureFile, generatedCodeBehindName);

            // ASSERT
            actual.Should().Be(expected);
        }
    }
}
