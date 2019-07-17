using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly IPlatformFactory _platformFactory;
        private readonly ICucumberMessageSink _cucumberMessageSink;
        private readonly IFieldValueProvider _fieldValueProvider;

        public CucumberMessageSender(ICucumberMessageFactory cucumberMessageFactory, IPlatformFactory platformFactory, ICucumberMessageSink cucumberMessageSink, IFieldValueProvider fieldValueProvider)
        {
            _cucumberMessageFactory = cucumberMessageFactory;
            _platformFactory = platformFactory;
            _cucumberMessageSink = cucumberMessageSink;
            _fieldValueProvider = fieldValueProvider;
        }

        public void SendTestRunStarted()
        {
            var nowDateAndTime = _fieldValueProvider.GetTestRunStartedTime();
            var testRunStartedMessageResult = _cucumberMessageFactory.BuildTestRunStartedMessage(nowDateAndTime);
            var envelope = _cucumberMessageFactory.BuildEnvelopeMessage(testRunStartedMessageResult);
            SendMessageOrThrowException(envelope);
        }

        public void SendTestCaseStarted(ScenarioInfo scenarioInfo)
        {
            var actualPickleId = _fieldValueProvider.GetTestCaseStartedPickleId(scenarioInfo);
            var nowDateAndTime = _fieldValueProvider.GetTestCaseStartedTime();

            var platform = _platformFactory.BuildFromSystemInformation();

            var testCaseStartedMessageResult = _cucumberMessageFactory.BuildTestCaseStartedMessage(actualPickleId, nowDateAndTime, platform);
            var envelope = _cucumberMessageFactory.BuildEnvelopeMessage(testCaseStartedMessageResult);
            SendMessageOrThrowException(envelope);
        }

        public void SendTestCaseFinished(ScenarioInfo scenarioInfo, TestResult testResult)
        {
            var actualPickleId = _fieldValueProvider.GetTestCaseFinishedPickleId(scenarioInfo);
            var nowDateAndTime = _fieldValueProvider.GetTestCaseFinishedTime();

            var testCaseFinishedMessageResult = _cucumberMessageFactory.BuildTestCaseFinishedMessage(actualPickleId, nowDateAndTime, testResult);
            var envelope = _cucumberMessageFactory.BuildEnvelopeMessage(testCaseFinishedMessageResult);
            SendMessageOrThrowException(envelope);
        }

        public void SendMessageOrThrowException(IResult<Envelope> messageResult)
        {
            switch (messageResult)
            {
                case ISuccess<Envelope> success:
                    _cucumberMessageSink.SendMessage(success.Result);
                    break;

                case WrappedFailure<Envelope> failure: throw new InvalidOperationException($"The message could not be created. {failure}");
                case ExceptionFailure<Envelope> failure: throw failure.Exception;
                case Failure<Envelope> failure: throw new InvalidOperationException($"The message could not be created. {failure.Description}");
                default: throw new InvalidOperationException("The message could not be created.");
            }
        }
    }
}
