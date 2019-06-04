using System;
using System.Globalization;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.EnvironmentAccess;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageSenderValueMockSource : ICucumberMessageSenderValueMockSource
    {
        private const string SpecFlowMessagesTestRunStartedTimeOverrideName = "SpecFlow_Messages_TestRunStartedTimeOverride";
        private const string SpecFlowMessagesTestCaseStartedTimeOverrideName = "SpecFlow_Messages_TestCaseStartedTimeOverride";
        private const string SpecFlowMessagesTestCaseStartedPickleIdOverrideName = "SpecFlow_Messages_TestCaseStartedPickleIdOverride";
        private const string SpecFlowMessagesTestCaseFinishedTimeOverrideName = "SpecFlow_Messages_TestCaseFinishedTimeOverride";
        private const string SpecFlowMessagesTestCaseFinishedPickleIdOverrideName = "SpecFlow_Messages_TestCaseFinishedPickleIdOverride";
        private readonly IEnvironmentWrapper _environmentWrapper;

        public CucumberMessageSenderValueMockSource(IEnvironmentWrapper environmentWrapper)
        {
            _environmentWrapper = environmentWrapper;
        }

        public DateTime? TryParseUniversalDateTime(string source)
        {
            return DateTime.TryParse(source, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var result)
                ? result
                : default;
        }

        public Guid? TryParseGuid(string source)
        {
            return Guid.TryParse(source, out var result) ? result : default(Guid?);
        }

        public DateTime? GetTestRunStartedTimeFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestRunStartedTimeOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            return TryParseUniversalDateTime(success.Result);
        }

        public DateTime? GetTestCaseStartedTimeFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseStartedTimeOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            return TryParseUniversalDateTime(success.Result);
        }

        public Guid? GetTestCaseStartedPickleIdFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseStartedPickleIdOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            return TryParseGuid(success.Result);
        }

        public DateTime? GetTestCaseFinishedTimeFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseFinishedTimeOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            return TryParseUniversalDateTime(success.Result);
        }

        public Guid? GetTestCaseFinishedPickleIdFromEnvironmentVariableOrNull()
        {
            if (!(_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseFinishedPickleIdOverrideName) is ISuccess<string> success))
            {
                return null;
            }

            return TryParseGuid(success.Result);
        }
    }
}
