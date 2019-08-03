using System;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

using static Io.Cucumber.Messages.TestCaseStarted.Types;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageFactory : ICucumberMessageFactory
    {
        public string ConvertToPickleIdString(Guid id)
        {
            return $"{id:D}";
        }

        public IResult<TestRunStarted> BuildTestRunStartedMessage(DateTime timeStamp)
        {
            if (timeStamp.Kind != DateTimeKind.Utc)
            {
                return Result<TestRunStarted>.Failure($"{nameof(timeStamp)} must be an UTC {nameof(DateTime)}. It is {timeStamp.Kind}");
            }

            var testRunStarted = new TestRunStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp)
            };

            return Result<TestRunStarted>.Success(testRunStarted);
        }

        public IResult<TestCaseStarted> BuildTestCaseStartedMessage(Guid pickleId, DateTime timeStamp, Platform platform)
        {
            if (platform is null)
            {
                return Result<TestCaseStarted>.Failure($"The {nameof(platform)} parameter must not be null");
            }

            if (timeStamp.Kind != DateTimeKind.Utc)
            {
                return Result<TestCaseStarted>.Failure($"{nameof(timeStamp)} must be an UTC {nameof(DateTime)}. It is {timeStamp.Kind}");
            }

            var testCaseStarted = new TestCaseStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp),
                PickleId = ConvertToPickleIdString(pickleId),
                Platform = platform
            };

            return Result<TestCaseStarted>.Success(testCaseStarted);
        }

        public IResult<TestCaseFinished> BuildTestCaseFinishedMessage(Guid pickleId, DateTime timeStamp, TestResult testResult)
        {
            if (testResult is null)
            {
                return Result<TestCaseFinished>.Failure(new ArgumentNullException(nameof(testResult)));
            }

            if (timeStamp.Kind != DateTimeKind.Utc)
            {
                return Result<TestCaseFinished>.Failure($"{nameof(timeStamp)} must be an UTC {nameof(DateTime)}. It is {timeStamp.Kind}");
            }

            var testCaseFinished = new TestCaseFinished
            {
                PickleId = ConvertToPickleIdString(pickleId),
                Timestamp = Timestamp.FromDateTime(timeStamp),
                TestResult = testResult
            };

            return Result<TestCaseFinished>.Success(testCaseFinished);
        }

        public IResult<Envelope> BuildEnvelopeMessage(IResult<TestRunStarted> testRunStarted)
        {
            switch (testRunStarted)
            {
                case ISuccess<TestRunStarted> success:
                    return Result<Envelope>.Success(new Envelope { TestRunStarted = success.Result });
                case IFailure failure:
                    return Result<Envelope>.Failure($"{nameof(testRunStarted)} must be an {nameof(ISuccess<TestRunStarted>)}.", failure);
                default:
                    return Result<Envelope>.Failure($"{nameof(testRunStarted)} must be an  {nameof(ISuccess<TestRunStarted>)}.");
            }
        }

        public IResult<Envelope> BuildEnvelopeMessage(IResult<TestCaseStarted> testCaseStarted)
        {
            switch (testCaseStarted)
            {
                case ISuccess<TestCaseStarted> success:
                    return Result<Envelope>.Success(new Envelope { TestCaseStarted = success.Result });
                case IFailure failure:
                    return Result<Envelope>.Failure($"{nameof(testCaseStarted)} must be an {nameof(ISuccess<TestCaseStarted>)}.", failure);
                default:
                    return Result<Envelope>.Failure($"{nameof(testCaseStarted)} must be an {nameof(ISuccess<TestCaseStarted>)}.");
            }
        }

        public IResult<Envelope> BuildEnvelopeMessage(IResult<TestCaseFinished> testCaseFinished)
        {
            switch (testCaseFinished)
            {
                case ISuccess<TestCaseFinished> success:
                    return Result<Envelope>.Success(new Envelope { TestCaseFinished = success.Result });
                case IFailure failure:
                    return Result<Envelope>.Failure($"{nameof(testCaseFinished)} must be an {nameof(ISuccess<TestCaseStarted>)}.", failure);
                default:
                    return Result<Envelope>.Failure($"{nameof(testCaseFinished)} must be an {nameof(ISuccess<TestCaseStarted>)}.");
            }
        }
    }
}
