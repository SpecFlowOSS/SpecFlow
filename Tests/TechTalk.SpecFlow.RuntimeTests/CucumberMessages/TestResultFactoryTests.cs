using System;
using FluentAssertions;
using Moq;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTests
    {
        [Fact(DisplayName = @"BuildFromScenarioContext should return a failure with an ArgumentNullException when null is passed")]
        public void BuildFromScenarioContext_Null_ShouldReturnFailureWithArgumentNullException()
        {
            // ARRANGE
            var testResultFactory = new TestResultFactory(new Mock<ITestResultPartsFactory>().Object);

            // ACT
            var actualTestResult = testResultFactory.BuildFromContext(null, null);

            // ASSERT
            actualTestResult.Should().BeAssignableTo<ExceptionFailure>().Which
                            .Exception.Should().BeOfType<ArgumentNullException>();
        }
    }
}
