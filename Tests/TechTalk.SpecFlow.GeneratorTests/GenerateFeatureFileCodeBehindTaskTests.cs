using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using SpecFlow.Tools.MsBuild.Generation;
using Xunit;

namespace TechTalk.SpecFlow.GeneratorTests
{
    public class GenerateFeatureFileCodeBehindTaskTests
    {
        [SkippableTheory(DisplayName = "FilePathGenerator should generate correct path (Windows).")]
        [InlineData(@"C:\temp\Project1", "Features", "Some.feature", "Some.feature.cs", @"C:\temp\Project1\Features\Some.feature.cs")]
        public void FilePathGenerator_GenerateFilePath_ShouldReturnCorrectPath_Windows(
            string projectDir,
            string relativeOutputDir,
            string featureFile,
            string generatedCodeBehindName,
            string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Windows));

            // ARRANGE
            var filePathGenerator = new FilePathGenerator();

            // ACT
            string actual = filePathGenerator.GenerateFilePath(projectDir, relativeOutputDir, featureFile, generatedCodeBehindName);

            // ASSERT
            actual.Should().Be(expected);
        }

        [SkippableTheory(DisplayName = "FilePathGenerator should generate correct path (Linux).")]
        [InlineData(@"/temp/Project1", "Features", "Some.feature", "Some.feature.cs", @"/temp/Project1/Features/Some.feature.cs")]
        public void FilePathGenerator_GenerateFilePath_ShouldReturnCorrectPath_Linux(
            string projectDir,
            string relativeOutputDir,
            string featureFile,
            string generatedCodeBehindName,
            string expected)
        {
            Skip.IfNot(RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX));

            // ARRANGE
            var filePathGenerator = new FilePathGenerator();

            // ACT
            string actual = filePathGenerator.GenerateFilePath(projectDir, relativeOutputDir, featureFile, generatedCodeBehindName);

            // ASSERT
            actual.Should().Be(expected);
        }
    }
}
