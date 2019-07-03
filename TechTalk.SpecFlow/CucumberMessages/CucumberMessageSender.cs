using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly ICucumberMessageSink _cucumberMessageSink;
        private readonly IFieldValueProvider _fieldValueProvider;

        public CucumberMessageSender(ICucumberMessageFactory cucumberMessageFactory, ICucumberMessageSink cucumberMessageSink, IFieldValueProvider fieldValueProvider)
        {
            _cucumberMessageFactory = cucumberMessageFactory;
            _cucumberMessageSink = cucumberMessageSink;
            _fieldValueProvider = fieldValueProvider;
        }

        public void SendTestRunStarted()
        {
            var nowDateAndTime = _fieldValueProvider.GetTestRunStartedTime();
            var testRunStartedMessageResult = _cucumberMessageFactory.BuildTestRunStartedMessage(nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testRunStartedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendTestCaseStarted(ScenarioInfo scenarioInfo)
        {
            var actualPickleId = _fieldValueProvider.GetTestCaseStartedPickleId(scenarioInfo);
            var nowDateAndTime = _fieldValueProvider.GetTestCaseStartedTime();

            var testCaseStartedMessageResult = _cucumberMessageFactory.BuildTestCaseStartedMessage(actualPickleId, nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testCaseStartedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendTestCaseFinished(ScenarioInfo scenarioInfo, TestResult testResult)
        {
            var actualPickleId = _fieldValueProvider.GetTestCaseFinishedPickleId(scenarioInfo);
            var nowDateAndTime = _fieldValueProvider.GetTestCaseFinishedTime();

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
