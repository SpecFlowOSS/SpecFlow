using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

using static Io.Cucumber.Messages.TestResult.Types;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ITestResultPartsFactory
    {
        IResult<TestResult> BuildPassedResult(ulong durationInNanoseconds);
        IResult<TestResult> BuildFailedResult(ulong durationInNanoseconds, ScenarioContext scenarioContext);
        IResult<TestResult> BuildAmbiguousResult(ulong durationInNanoseconds, ScenarioContext scenarioContext);
        IResult<TestResult> BuildPendingResult(ulong durationInNanoseconds, ScenarioContext scenarioContext);
        IResult<TestResult> BuildSkippedResult(ulong durationInNanoseconds);
        IResult<TestResult> BuildUndefinedResult(ulong durationInNanoseconds, ScenarioContext scenarioContext, FeatureContext featureContext);
    }

    public class TestResultPartsFactory : ITestResultPartsFactory
    {
        private readonly ITestErrorMessageFactory _testErrorMessageFactory;
        private readonly ITestPendingMessageFactory _testPendingMessageFactory;
        private readonly ITestAmbiguousMessageFactory _testAmbiguousMessageFactory;
        private readonly ITestUndefinedMessageFactory _testUndefinedMessageFactory;

        public TestResultPartsFactory(ITestErrorMessageFactory testErrorMessageFactory, ITestPendingMessageFactory testPendingMessageFactory, ITestAmbiguousMessageFactory testAmbiguousMessageFactory, ITestUndefinedMessageFactory testUndefinedMessageFactory)
        {
            _testErrorMessageFactory = testErrorMessageFactory;
            _testPendingMessageFactory = testPendingMessageFactory;
            _testAmbiguousMessageFactory = testAmbiguousMessageFactory;
            _testUndefinedMessageFactory = testUndefinedMessageFactory;
        }


        public IResult<TestResult> BuildPassedResult(ulong durationInNanoseconds)
        {
            return BuildTestResult(durationInNanoseconds, Status.Passed, "");
        }

        public IResult<TestResult> BuildFailedResult(ulong durationInNanoseconds, ScenarioContext scenarioContext)
        {
            return BuildTestResult(durationInNanoseconds, Status.Failed, _testErrorMessageFactory.BuildFromScenarioContext(scenarioContext));
        }

        public IResult<TestResult> BuildAmbiguousResult(ulong durationInNanoseconds, ScenarioContext scenarioContext)
        {
            return BuildTestResult(durationInNanoseconds, Status.Ambiguous, _testAmbiguousMessageFactory.BuildFromScenarioContext(scenarioContext));
        }

        public IResult<TestResult> BuildPendingResult(ulong durationInNanoseconds, ScenarioContext scenarioContext)
        {
            return BuildTestResult(durationInNanoseconds, Status.Pending, _testPendingMessageFactory.BuildFromScenarioContext(scenarioContext));
        }

        public IResult<TestResult> BuildSkippedResult(ulong durationInNanoseconds)
        {
            return BuildTestResult(durationInNanoseconds, Status.Skipped, "");
        }

        public IResult<TestResult> BuildUndefinedResult(ulong durationInNanoseconds, ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            return BuildTestResult(durationInNanoseconds, Status.Undefined, _testUndefinedMessageFactory.BuildFromContext(scenarioContext, featureContext));
        }

        internal IResult<TestResult> BuildTestResult(ulong durationInNanoseconds, Status status, string message)
        {
            var testResult = new TestResult
            {
                DurationNanoseconds = durationInNanoseconds,
                Status = status,
                Message = message ?? ""
            };

            return Result<TestResult>.Success(testResult);
        }

       
    }


    public class TestResultFactory : ITestResultFactory
    {
        private readonly ITestResultPartsFactory _testResultPartsFactory;

        public TestResultFactory(ITestResultPartsFactory testResultPartsFactory)
        {
            _testResultPartsFactory = testResultPartsFactory;
        }

        internal ulong ConvertTicksToPositiveNanoseconds(long ticks)
        {
            ulong ticksOrZero = (ulong)Math.Min(ticks, 0);
            return ticksOrZero * 100;
        }

        public IResult<TestResult> BuildFromContext(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            if (scenarioContext is null)
            {
                return Result<TestResult>.Failure(new ArgumentNullException(nameof(scenarioContext)));
            }

            ulong nanoseconds = ConvertTicksToPositiveNanoseconds(scenarioContext.Stopwatch.Elapsed.Ticks);
            switch (scenarioContext.ScenarioExecutionStatus)
            {
                case ScenarioExecutionStatus.OK: return _testResultPartsFactory.BuildPassedResult(nanoseconds);
                case ScenarioExecutionStatus.TestError: return _testResultPartsFactory.BuildFailedResult(nanoseconds, scenarioContext);
                case ScenarioExecutionStatus.StepDefinitionPending: return _testResultPartsFactory.BuildPendingResult(nanoseconds, scenarioContext);
                case ScenarioExecutionStatus.BindingError: return _testResultPartsFactory.BuildAmbiguousResult(nanoseconds, scenarioContext);
                case ScenarioExecutionStatus.UndefinedStep: return _testResultPartsFactory.BuildUndefinedResult(nanoseconds, scenarioContext, featureContext);
                case ScenarioExecutionStatus.Skipped: return _testResultPartsFactory.BuildSkippedResult(nanoseconds);
                default: return Result<TestResult>.Failure($"Status '{scenarioContext.ScenarioExecutionStatus}' is unknown or not supported.");
            }
        }
    }

    
}

