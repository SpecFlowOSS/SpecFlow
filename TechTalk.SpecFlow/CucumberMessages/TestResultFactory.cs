using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class TestResultFactory : ITestResultFactory
    {
        private readonly ITestErrorMessageFactory _testErrorMessageFactory;
        private readonly ITestPendingMessageFactory _testPendingMessageFactory;
        private readonly ITestAmbiguousMessageFactory _testAmbiguousMessageFactory;

        public TestResultFactory(ITestErrorMessageFactory testErrorMessageFactory, ITestPendingMessageFactory testPendingMessageFactory, ITestAmbiguousMessageFactory testAmbiguousMessageFactory)
        {
            _testErrorMessageFactory = testErrorMessageFactory;
            _testPendingMessageFactory = testPendingMessageFactory;
            _testAmbiguousMessageFactory = testAmbiguousMessageFactory;
        }

        public ulong ConvertTicksToPositiveNanoseconds(long ticks)
        {
            ulong ticksOrZero = (ulong)Math.Min(ticks, 0);
            return ticksOrZero * 100;
        }

        public IResult<TestResult> BuildPassedResult(ulong durationInNanoseconds)
        {
            return BuildTestResult(durationInNanoseconds, Status.Passed, "");
        }

        public IResult<TestResult> BuildFailedResult(ulong durationInNanoseconds, string message)
        {
            return BuildTestResult(durationInNanoseconds, Status.Failed, message);
        }

        public IResult<TestResult> BuildAmbiguousResult(ulong durationInNanoseconds, string message)
        {
            return BuildTestResult(durationInNanoseconds, Status.Ambiguous, message);
        }

        public IResult<TestResult> BuildPendingResult(ulong durationInNanoseconds, string message)
        {
            return BuildTestResult(durationInNanoseconds, Status.Pending, message);
        }

        public IResult<TestResult> BuildSkippedResult(ulong durationInNanoseconds, string message)
        {
            return BuildTestResult(durationInNanoseconds, Status.Skipped, message);
        }

        public IResult<TestResult> BuildUndefinedResult(ulong durationInNanoseconds, string message)
        {
            return BuildTestResult(durationInNanoseconds, Status.Undefined, message);
        }

        public IResult<TestResult> BuildTestResult(ulong durationInNanoseconds, Status status, string message)
        {
            var testResult = new TestResult
            {
                DurationNanoseconds = durationInNanoseconds,
                Status = status,
                Message = message ?? ""
            };

            return Result<TestResult>.Success(testResult);
        }

        public IResult<TestResult> BuildFromScenarioContext(ScenarioContext scenarioContext)
        {
            if (scenarioContext is null)
            {
                return Result<TestResult>.Failure(new ArgumentNullException(nameof(scenarioContext)));
            }

            ulong nanoseconds = ConvertTicksToPositiveNanoseconds(scenarioContext.Stopwatch.Elapsed.Ticks);
            switch (scenarioContext.ScenarioExecutionStatus)
            {
                case ScenarioExecutionStatus.OK: return BuildPassedResult(nanoseconds);
                case ScenarioExecutionStatus.TestError: return BuildFailedResult(nanoseconds, _testErrorMessageFactory.BuildFromScenarioContext(scenarioContext));
                case ScenarioExecutionStatus.StepDefinitionPending: return BuildPendingResult(nanoseconds, _testPendingMessageFactory.BuildFromScenarioContext(scenarioContext));
                case ScenarioExecutionStatus.BindingError: return BuildAmbiguousResult(nanoseconds, _testAmbiguousMessageFactory.BuildFromScenarioContext(scenarioContext));
                default: return Result<TestResult>.Failure($"Status '{scenarioContext.ScenarioExecutionStatus}' is unknown or not supported.");
            }
        }
    }
}
