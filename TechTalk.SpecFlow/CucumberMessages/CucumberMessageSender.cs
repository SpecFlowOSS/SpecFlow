using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly IClock _clock;
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly ICucumberMessageSink _cucumberMessageSink;
        private readonly ICucumberMessageSenderValueMockSource _cucumberMessageSenderValueMockSource;

        public CucumberMessageSender(IClock clock, ICucumberMessageFactory cucumberMessageFactory, ICucumberMessageSink cucumberMessageSink, ICucumberMessageSenderValueMockSource cucumberMessageSenderValueMockSource)
        {
            _clock = clock;
            _cucumberMessageFactory = cucumberMessageFactory;
            _cucumberMessageSink = cucumberMessageSink;
            _cucumberMessageSenderValueMockSource = cucumberMessageSenderValueMockSource;
        }

        public DateTime GetTimeStamp(Func<DateTime?> mockSource)
        {
            var timeFromEnvironmentResult = mockSource();
            var now = _clock.GetNowDateAndTime();
            return timeFromEnvironmentResult ?? now;
        }

        public Guid GetPickleId(Func<Guid?> mockSource, Guid passedPickleId)
        {
            var overridePickleId = mockSource();
            return overridePickleId ?? passedPickleId;
        }

        public void SendTestRunStarted()
        {
            var nowDateAndTime = GetTimeStamp(_cucumberMessageSenderValueMockSource.GetTestRunStartedTimeFromEnvironmentVariableOrNull);
            var testRunStartedMessageResult = _cucumberMessageFactory.BuildTestRunStartedMessage(nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testRunStartedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendTestCaseStarted(Guid pickleId)
        {
            var actualPickleId = GetPickleId(_cucumberMessageSenderValueMockSource.GetTestCaseStartedPickleIdFromEnvironmentVariableOrNull, pickleId);

            var nowDateAndTime = GetTimeStamp(_cucumberMessageSenderValueMockSource.GetTestCaseStartedTimeFromEnvironmentVariableOrNull);
            var testCaseStartedMessageResult = _cucumberMessageFactory.BuildTestCaseStartedMessage(actualPickleId, nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testCaseStartedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendTestCaseFinished(Guid pickleId, TestResult testResult)
        {
            var actualPickleId = GetPickleId(_cucumberMessageSenderValueMockSource.GetTestCaseFinishedPickleIdFromEnvironmentVariableOrNull, pickleId);

            var nowDateAndTime = GetTimeStamp(_cucumberMessageSenderValueMockSource.GetTestCaseFinishedTimeFromEnvironmentVariableOrNull);
            var testCaseFinishedMessageResult = _cucumberMessageFactory.BuildTestCaseFinishedMessage(actualPickleId, nowDateAndTime, testResult);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testCaseFinishedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendMessageOrThrowException(IResult<Wrapper> messageResult)
        {
            switch (messageResult)
            {
                case ISuccess<Wrapper> success:
                    _cucumberMessageSink.SendMessage(success.Result);
                    break;

                case WrappedFailure<Wrapper> failure: throw new InvalidOperationException($"The message could not be created. {failure}");
                case ExceptionFailure<Wrapper> failure: throw failure.Exception;
                case Failure<Wrapper> failure: throw new InvalidOperationException($"The message could not be created. {failure.Description}");
                default: throw new InvalidOperationException("The message could not be created.");
            }
        }
    }
}
