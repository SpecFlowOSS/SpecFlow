using System;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageFactory : ICucumberMessageFactory
    {
        private const string UsedCucumberImplementationString = @"SpecFlow";

        public TestRunStarted BuildTestRunStartedMessage(DateTime timeStamp)
        {
            return new TestRunStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp),
                CucumberImplementation = UsedCucumberImplementationString
            };
        }

        public Result<TestCaseStarted> BuildTestCaseStartedMessage(string pickleId, DateTime timeStamp)
        {
            if (timeStamp.Kind != DateTimeKind.Utc)
            {
                return Result<TestCaseStarted>.Failure();
            }

            var testCaseStarted = new TestCaseStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp),
                PickleId = pickleId
            };

            return Result<TestCaseStarted>.Success(testCaseStarted);
        }
    }
}
