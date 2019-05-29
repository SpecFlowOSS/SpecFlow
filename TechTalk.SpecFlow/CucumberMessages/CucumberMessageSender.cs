using System;
using System.Globalization;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.EnvironmentAccess;
using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSender : ICucumberMessageSender
    {
        private const string SpecFlowMessagesTestRunStartedTimeOverrideName = "SpecFlow_Messages_TestRunStartedTimeOverride";
        private const string SpecFlowMessagesTestCaseStartedTimeOverrideName = "SpecFlow_Messages_TestCaseStartedTimeOverride";
        private const string SpecFlowMessagesTestCaseStartedPickleIdOverrideName = "SpecFlow_Messages_TestCaseStartedPickleIdOverride";
        private readonly IClock _clock;
        private readonly ICucumberMessageFactory _cucumberMessageFactory;
        private readonly ICucumberMessageSink _cucumberMessageSink;
        private readonly IEnvironmentWrapper _environmentWrapper;

        public CucumberMessageSender(IClock clock, ICucumberMessageFactory cucumberMessageFactory, ICucumberMessageSink cucumberMessageSink, IEnvironmentWrapper environmentWrapper)
        {
            _clock = clock;
            _cucumberMessageFactory = cucumberMessageFactory;
            _cucumberMessageSink = cucumberMessageSink;
            _environmentWrapper = environmentWrapper;
        }

        public DateTime? GetTestRunStartedTimeFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestRunStartedTimeOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            if (!DateTime.TryParse(success.Result, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var result))
            {
                return null;
            }

            return result;
        }

        public DateTime? GetTestCaseStartedTimeFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseStartedTimeOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            if (!DateTime.TryParse(success.Result, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var result))
            {
                return null;
            }

            return result;
        }

        public Guid? GetTestCaseStartedPickleIdFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseStartedPickleIdOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            if (!Guid.TryParse(success.Result, out var result))
            {
                return null;
            }

            return result;
        }

        public void SendTestRunStarted()
        {
            var timeFromEnvironmentResult = GetTestRunStartedTimeFromEnvironmentVariableOrNull();
            var now = _clock.GetNowDateAndTime();
            var nowDateAndTime = timeFromEnvironmentResult ?? now;
            var testRunStartedMessageResult = _cucumberMessageFactory.BuildTestRunStartedMessage(nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testRunStartedMessageResult);
            SendMessageOrThrowException(wrapper);
        }

        public void SendTestCaseStarted(Guid pickleId)
        {
            var overridePickleId = GetTestCaseStartedPickleIdFromEnvironmentVariableOrNull();
            pickleId = overridePickleId ?? pickleId;

            var timeFromEnvironmentResult = GetTestCaseStartedTimeFromEnvironmentVariableOrNull();
            var now = _clock.GetNowDateAndTime();
            var nowDateAndTime = timeFromEnvironmentResult ?? now;
            var testCaseStartedMessageResult = _cucumberMessageFactory.BuildTestCaseStartedMessage(pickleId, nowDateAndTime);
            var wrapper = _cucumberMessageFactory.BuildWrapperMessage(testCaseStartedMessageResult);
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
