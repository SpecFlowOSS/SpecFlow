using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
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