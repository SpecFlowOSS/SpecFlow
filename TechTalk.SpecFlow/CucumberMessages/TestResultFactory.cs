using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class TestResultFactory : ITestResultFactory
    {
        public IResult<TestResult> BuildPassedResult(ulong durationInNanoseconds, string message)
        {
            return BuildTestResult(durationInNanoseconds, Status.Passed, message);
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
    }
}
