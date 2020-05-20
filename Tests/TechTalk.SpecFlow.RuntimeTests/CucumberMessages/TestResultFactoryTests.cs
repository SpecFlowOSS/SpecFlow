using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentAssertions;
using Io.Cucumber.Messages;
using Moq;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class TestResultFactoryTests
    {
        private ScenarioContext CreateScenarioContext(ScenarioExecutionStatus scenarioExecutionStatus)
        {
            return new ScenarioContext(null, new ScenarioInfo("","", null, null), null)
            {
                ScenarioExecutionStatus = scenarioExecutionStatus
            };
        }

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


        private static Dictionary<ScenarioExecutionStatus, Expression<Action<ITestResultPartsFactory>>> _testCases = new Dictionary<ScenarioExecutionStatus, Expression<Action<ITestResultPartsFactory>>>()
        {
            { ScenarioExecutionStatus.OK, (factory => factory.BuildPassedResult(It.IsAny<ulong>()))},
            { ScenarioExecutionStatus.BindingError, (factory => factory.BuildAmbiguousResult(It.IsAny<ulong>(), It.IsAny<ScenarioContext>()))},
            { ScenarioExecutionStatus.StepDefinitionPending, (factory => factory.BuildPendingResult(It.IsAny<ulong>(), It.IsAny<ScenarioContext>()))},
            { ScenarioExecutionStatus.TestError, (factory => factory.BuildFailedResult(It.IsAny<ulong>(), It.IsAny<ScenarioContext>()))},
            { ScenarioExecutionStatus.UndefinedStep, (factory => factory.BuildUndefinedResult(It.IsAny<ulong>(), It.IsAny<ScenarioContext>(), It.IsAny<FeatureContext>()))},
            { ScenarioExecutionStatus.Skipped, (factory => factory.BuildSkippedResult(It.IsAny<ulong>()))},
        };

        public static IEnumerable<object[]> GetTestCases
        {
            get
            {
                foreach (var func in _testCases)
                {
                    yield return new object[] {func.Key, func.Value};
                }
            }
            
        }

        [Theory(DisplayName = "Correct method on TestResultPartsFactory is called for ScenarioExecutionStatus")]
        [Xunit.MemberData(nameof(GetTestCases))]
        
        public void BuildFromContext_PassedScenario_TestResultPartFactoryBuildPassedResultIsCalled(ScenarioExecutionStatus scenarioExecutionStatus, Expression<Action<ITestResultPartsFactory>> expression)
        {
            // ARRANGE
            var testResultPartsFactoryMock = new Mock<ITestResultPartsFactory>();
            var testResultFactory = new TestResultFactory(testResultPartsFactoryMock.Object);

            // ACT
            var actualTestResult = testResultFactory.BuildFromContext(CreateScenarioContext(scenarioExecutionStatus), null);

            // ASSERT
             testResultPartsFactoryMock.Verify(expression, Times.Once);
        }

        [Fact(DisplayName = "Correct method on TestResultPartsFactory is called for failed Scenario")]
        public void BuildFromContext_FailedScenario_TestResultPartFactoryBuildPassedResultIsCalled()
        {
            // ARRANGE
            var testResultPartsFactoryMock = new Mock<ITestResultPartsFactory>();
            var testResultFactory = new TestResultFactory(testResultPartsFactoryMock.Object);

            // ACT
            var actualTestResult = testResultFactory.BuildFromContext(CreateScenarioContext(ScenarioExecutionStatus.OK), null);

            // ASSERT
            testResultPartsFactoryMock.Verify(m => m.BuildPassedResult(It.IsAny<ulong>()), Times.Once);
        }
    }
}
