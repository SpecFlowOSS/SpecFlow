using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Xunit;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace TechTalk.SpecFlow.GeneratorTests
{
    
    public class TestGeneratorErrorsTests : TestGeneratorTestsBase
    {
        [Fact]
        public void Should_succeed_when_empty_feature_file()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings);

            var result = testGenerator.GenerateTestFile(CreateSimpleFeatureFileInput(""), defaultSettings);
            result.Success.Should().Be(true);
        }

        [Fact]
        public void Should_succeed_when_feature_file_with_only_whitespace()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings);

            var result = testGenerator.GenerateTestFile(CreateSimpleFeatureFileInput(" "), defaultSettings);
            result.Success.Should().Be(true);
        }

        [Fact]
        public void Should_not_report_error_when_empty_feature_file()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings);

            var result = testGenerator.GenerateTestFile(CreateSimpleFeatureFileInput(""), defaultSettings);
            result.Errors.Should().BeNull();
        }

        [Fact]
        public void Should_not_report_error_when_feature_file_with_only_whitespace()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings);

            var result = testGenerator.GenerateTestFile(CreateSimpleFeatureFileInput(" "), defaultSettings);
            result.Errors.Should().BeNull();
        }

        [Fact]
        public void Should_not_succeed_when_invalid_feature_file()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings);

            var result = testGenerator.GenerateTestFile(CreateSimpleInvalidFeatureFileInput(), defaultSettings);
            result.Success.Should().Be(false);
        }

        [Fact]
        public void Should_report_error_when_invalid_feature_file()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings);

            var result = testGenerator.GenerateTestFile(CreateSimpleInvalidFeatureFileInput(), defaultSettings);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void Should_report_multiple_errors_when_feature_file_contains_such()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings); 

            var result = testGenerator.GenerateTestFile(CreateSimpleFeatureFileInput(@"
                Feature: Addition
                Scenario: Add two numbers
	                Given I have entered 50 into the calculator
                    AndXXX the keyword is misspelled
                    AndYYY this keyword is also misspelled"), defaultSettings);
            result.Errors.Should().NotBeNull();
            result.Errors.Count().Should().Be(2);
        }

        [Fact]
        public void Should_report_error_when_unsupported_project_language()
        {
            ProjectSettings invalidLangSettings = new ProjectSettings { ProjectFolder = Path.GetTempPath(), ProjectPlatformSettings = new ProjectPlatformSettings { Language = "InvalidLang" } };
            var testGenerator = CreateTestGenerator(invalidLangSettings); 

            var result = testGenerator.GenerateTestFile(CreateSimpleValidFeatureFileInput(), defaultSettings);
            result.Errors.Should().NotBeNull();
            result.Errors.Should().NotBeEmpty();
        }

        [Fact]
        public void Should_report_error_when_the_scenario_has_no_title()
        {
            var testGenerator = CreateTestGenerator(net35CSProjectSettings);

            var result = testGenerator.GenerateTestFile(CreateSimpleFeatureFileInput(@"
                Feature: Addition
                Scenario:
	                Given I have entered 50 into the calculator"), defaultSettings);
            result.Errors.Should().NotBeNull();
            result.Errors.Count().Should().Be(1);
            result.Errors.First().Message.Should().ContainEquivalentOf("scenario");
            result.Errors.First().Message.Should().ContainEquivalentOf("title");
        }
    }
}
