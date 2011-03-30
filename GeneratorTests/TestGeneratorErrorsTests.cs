using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using TechTalk.SpecFlow.Generator;
using TechTalk.SpecFlow.Generator.Configuration;
using Should;
using TechTalk.SpecFlow.Generator.Interfaces;

namespace GeneratorTests
{
    [TestFixture]
    public class TestGeneratorErrorsTests : TestGeneratorTestsBase
    {
        [Test]
        public void Should_not_succeed_when_invalid_feature_file()
        {
            TestGenerator testGenerator = new TestGenerator(new GeneratorConfiguration());

            var result = testGenerator.GenerateTestFile(CreateSimpleInvalidFeatureFileInput(), net35CSSettings);
            result.Success.ShouldBeFalse();
        }

        [Test]
        public void Should_report_error_when_invalid_feature_file()
        {
            TestGenerator testGenerator = new TestGenerator(new GeneratorConfiguration());

            var result = testGenerator.GenerateTestFile(CreateSimpleInvalidFeatureFileInput(), net35CSSettings);
            result.Errors.ShouldNotBeNull();
            result.Errors.ShouldNotBeEmpty();
        }

        [Test]
        public void Should_report_multiple_errors_when_feature_file_contains_such()
        {
            TestGenerator testGenerator = new TestGenerator(new GeneratorConfiguration());

            var result = testGenerator.GenerateTestFile(CreateSimpleFeatureFileInput(@"
                Feature: Addition
                Scenario: Add two numbers
	                Given I have entered 50 into the calculator
                    AndXXX the keyword is misspelled
                    AndYYY this keyword is also misspelled"), net35CSSettings);
            result.Errors.ShouldNotBeNull();
            result.Errors.Count().ShouldEqual(2);
        }

        [Test]
        public void Should_report_error_when_invalid_input()
        {
            TestGenerator testGenerator = new TestGenerator(new GeneratorConfiguration());

            var result = testGenerator.GenerateTestFile(CreateSimpleValidFeatureFileInput(), 
                new GenerationSettings { TargetLanguage = "InvalidLang"});
            result.Errors.ShouldNotBeNull();
            result.Errors.ShouldNotBeEmpty();
        }
    }
}
