using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using static Io.Cucumber.Messages.TestResult.Types;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class TestResultPartsFactory : ITestResultPartsFactory
    {
        private readonly ITestAmbiguousMessageFactory _testAmbiguousMessageFactory;
        private readonly ITestErrorMessageFactory _testErrorMessageFactory;
        private readonly ITestPendingMessageFactory _testPendingMessageFactory;
        private readonly ITestUndefinedMessageFactory _testUndefinedMessageFactory;

        public TestResultPartsFactory(ITestErrorMessageFactory testErrorMessageFactory, ITestPendingMessageFactory testPendingMessageFactory, ITestAmbiguousMessageFactory testAmbiguousMessageFactory,
            ITestUndefinedMessageFactory testUndefinedMessageFactory)
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
}