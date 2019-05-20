using System;
using Google.Protobuf;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private readonly IClock _clock;
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly ICucumberMessageSink _cucumberMessageSink;

        public CucumberMessageSender(IClock clock, ICucumberMessageFactory cucumberMessageFactory, ICucumberMessageSink cucumberMessageSink)
        {
            _clock = clock;
            _cucumberMessageFactory = cucumberMessageFactory;
            _cucumberMessageSink = cucumberMessageSink;
        }

        public void SendTestRunStarted()
        {
            var testRunStartedMessageResult = _cucumberMessageFactory.BuildTestRunStartedMessage(_clock.GetNowDateAndTime());
            SendMessageOrThrowException(testRunStartedMessageResult);
        }

        public void SendTestCaseStarted(string pickleId)
        {
            var testCaseStartedMessageResult = _cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, _clock.GetNowDateAndTime());
            SendMessageOrThrowException(testCaseStartedMessageResult);
        }

        public void SendMessageOrThrowException(Result messageResult)
        {
            switch (messageResult)
            {
                case ISuccess<IMessage> success:
                    _cucumberMessageSink.SendMessage(success.Result);
                    break;

                case ExceptionFailure failure: throw failure.Exception;
                default: throw new InvalidOperationException("The message could not be created.");
            }
        }
    }
}
