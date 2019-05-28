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
                return Result<TestRunStarted>.Failure($"{nameof(timeStamp)} must be a UTC {nameof(DateTime)}. It is {timeStamp.Kind}");
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
                return Result<TestCaseStarted>.Failure($"{nameof(timeStamp)} must be a UTC {nameof(DateTime)}. It is {timeStamp.Kind}");
            }

            var testCaseStarted = new TestCaseStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp),
                PickleId = pickleId.ToString("D")
            };

            return Result<TestCaseStarted>.Success(testCaseStarted);
        }

        public IResult<Wrapper> BuildWrapperMessage(IResult<TestRunStarted> testRunStarted)
        {
            if (!(testRunStarted is ISuccess<TestRunStarted> success))
            {
                switch (testRunStarted)
                {
                    case Failure failure: return Result<Wrapper>.Failure($"{nameof(testRunStarted)} must be an ISuccess.", failure);
                    default: return Result<Wrapper>.Failure($"{nameof(testRunStarted)} must be an ISuccess.");
                }
            }

            var wrapper = new Wrapper { TestRunStarted = success.Result };
            return Result<Wrapper>.Success(wrapper);
        }

        public IResult<Wrapper> BuildWrapperMessage(IResult<TestCaseStarted> testCaseStarted)
        {
            if (!(testCaseStarted is ISuccess<TestCaseStarted> success))
            {
                return Result<Wrapper>.Failure($"{nameof(testCaseStarted)} must be an ISuccess.");
            }

            var wrapper = new Wrapper { TestCaseStarted = success.Result };
            return Result<Wrapper>.Success(wrapper);
        }
    }
}
