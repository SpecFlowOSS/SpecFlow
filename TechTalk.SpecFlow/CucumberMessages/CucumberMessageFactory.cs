using System;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageFactory : ICucumberMessageFactory
    {
        private const string UsedCucumberImplementationString = @"SpecFlow";

        public IResult<TestRunStarted> BuildTestRunStartedMessage(DateTime timeStamp)
        {
            if (timeStamp.Kind != DateTimeKind.Utc)
            {
                return Result<TestRunStarted>.Failure();
            }

            var testRunStarted = new TestRunStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp),
                CucumberImplementation = UsedCucumberImplementationString
            };

            return Result<TestRunStarted>.Success(testRunStarted);
        }

        public IResult<TestCaseStarted> BuildTestCaseStartedMessage(Guid pickleId, DateTime timeStamp)
        {
            if (timeStamp.Kind != DateTimeKind.Utc)
            {
                return Result<TestCaseStarted>.Failure();
            }

            var testCaseStarted = new TestCaseStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp),
                PickleId = pickleId.ToString("D")
            };

            return Result<TestCaseStarted>.Success(testCaseStarted);
        }
    }
}
