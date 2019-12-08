using System;
using System.Collections.Generic;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly IPlatformFactory _platformFactory;
        private readonly IFieldValueProvider _fieldValueProvider;
        private readonly ITestRunResultSuccessCalculator _testRunResultSuccessCalculator;

        private readonly List<ICucumberMessageSink> _cucumberMessageSinks;

        public CucumberMessageSender(
            ICucumberMessageFactory cucumberMessageFactory,
            IPlatformFactory platformFactory,
            IFieldValueProvider fieldValueProvider,
            ITestRunResultSuccessCalculator testRunResultSuccessCalculator, ISinkProvider sinkProvider)
        {
            _cucumberMessageFactory = cucumberMessageFactory ?? throw new ArgumentNullException(nameof(cucumberMessageFactory));
            _platformFactory = platformFactory;
            _fieldValueProvider = fieldValueProvider;
            _testRunResultSuccessCalculator = testRunResultSuccessCalculator;
            _cucumberMessageSinks = sinkProvider.GetMessageSinksFromConfiguration();
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

        public void SendTestRunFinished(TestRunResult testRunResult)
        {
            var nowDateAndTime = _fieldValueProvider.GetTestRunFinishedTime();
            bool isSuccess = _testRunResultSuccessCalculator.IsSuccess(testRunResult);

            var testRunFinishedMessage = _cucumberMessageFactory.BuildTestRunFinishedMessage(isSuccess, nowDateAndTime);
            var envelope = _cucumberMessageFactory.BuildEnvelopeMessage(testRunFinishedMessage);
            SendMessageOrThrowException(envelope);
        }

        public void SendMessageOrThrowException(IResult<Envelope> messageResult)
        {
            switch (messageResult)
            {
                case ISuccess<Envelope> success:

                    foreach (var cucumberMessageSink in _cucumberMessageSinks)
                    {
                        cucumberMessageSink.SendMessage(success.Result);
                    }
                    break;

                case WrappedFailure<Envelope> failure: throw new InvalidOperationException($"The message could not be created. {failure}");
                case ExceptionFailure<Envelope> failure: throw failure.Exception;
                case Failure<Envelope> failure: throw new InvalidOperationException($"The message could not be created. {failure.Description}");
                default: throw new InvalidOperationException("The message could not be created.");
            }
        }
    }
}
