using System;
using FluentAssertions;
using TechTalk.SpecFlow.CommonModels;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTests : TestResultFactoryTestsBase
    {
        [Fact(DisplayName = @"BuildFromScenarioContext should return a failure with an ArgumentNullException when null is passed")]
        public void BuildFromScenarioContext_Null_ShouldReturnFailureWithArgumentNullException()
        {
            // ARRANGE
            var testResultFactory = GetTestResultFactory();

            // ACT
            var actualTestResult = testResultFactory.BuildFromContext(null, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ExceptionFailure>().Which
                            .Exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
