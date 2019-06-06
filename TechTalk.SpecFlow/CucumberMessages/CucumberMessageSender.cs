using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly ICucumberMessageSink _cucumberMessageSink;
        private readonly ICucumberMessageSenderValueMockSource _cucumberMessageSenderValueMockSource;

        public CucumberMessageSender(ICucumberMessageFactory cucumberMessageFactory, ICucumberMessageSink cucumberMessageSink, ICucumberMessageSenderValueMockSource cucumberMessageSenderValueMockSource)
        {
            _cucumberMessageFactory = cucumberMessageFactory;
            _cucumberMessageSink = cucumberMessageSink;
            _cucumberMessageSenderValueMockSource = cucumberMessageSenderValueMockSource;
        }

        public Guid GetPickleId(Func<Guid?> mockSource, Guid passedPickleId)
        {
            var overridePickleId = mockSource();
            return overridePickleId ?? passedPickleId;
        }

        public void SendTestRunStarted()
        {
            var nowDateAndTime = _cucumberMessageSenderValueMockSource.GetTestRunStartedTime();
            var testRunStartedMessageResult = _cucumberMessageFactory.BuildTestRunStartedMessage(nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testRunStartedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendTestCaseStarted(ScenarioInfo scenarioInfo)
        {
            var actualPickleId = _cucumberMessageSenderValueMockSource.GetTestCaseStartedPickleId(scenarioInfo);
            var nowDateAndTime = _cucumberMessageSenderValueMockSource.GetTestCaseStartedTime();

            var testCaseStartedMessageResult = _cucumberMessageFactory.BuildTestCaseStartedMessage(actualPickleId, nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testCaseStartedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendTestCaseFinished(ScenarioInfo scenarioInfo, TestResult testResult)
        {
            var actualPickleId = _cucumberMessageSenderValueMockSource.GetTestCaseFinishedPickleId(scenarioInfo);
            var nowDateAndTime = _cucumberMessageSenderValueMockSource.GetTestCaseFinishedTime();

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
