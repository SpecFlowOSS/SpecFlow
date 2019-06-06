using System;
using System.Globalization;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.EnvironmentAccess;
using TechTalk.SpecFlow.Time;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class FieldValueProvider : IFieldValueProvider
    {
        private const string SpecFlowMessagesTestRunStartedTimeOverrideName = "SpecFlow_Messages_TestRunStartedTimeOverride";
        private const string SpecFlowMessagesTestCaseStartedTimeOverrideName = "SpecFlow_Messages_TestCaseStartedTimeOverride";
        private const string SpecFlowMessagesTestCaseStartedPickleIdOverrideName = "SpecFlow_Messages_TestCaseStartedPickleIdOverride";
        private const string SpecFlowMessagesTestCaseFinishedTimeOverrideName = "SpecFlow_Messages_TestCaseFinishedTimeOverride";
        private const string SpecFlowMessagesTestCaseFinishedPickleIdOverrideName = "SpecFlow_Messages_TestCaseFinishedPickleIdOverride";
        private readonly IEnvironmentWrapper _environmentWrapper;
        private readonly IClock _clock;
        private readonly IPickleIdStore _pickleIdStore;

        public FieldValueProvider(IEnvironmentWrapper environmentWrapper, IClock clock, IPickleIdStore pickleIdStore)
        {
            _environmentWrapper = environmentWrapper;
            _clock = clock;
            _pickleIdStore = pickleIdStore;
        }

        public bool TryParseUniversalDateTime(string source, out DateTime result)
        {
            return DateTime.TryParse(source, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out result);
        }

        public DateTime GetTestRunStartedTime()
        {
            if (_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestRunStartedTimeOverrideName) is ISuccess<string> success
                && TryParseUniversalDateTime(success.Result, out var dateTime))
            {
                return dateTime;
            }

            return _clock.GetNowDateAndTime();
        }

        public DateTime GetTestCaseStartedTime()
        {
            if (_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseStartedTimeOverrideName) is ISuccess<string> success
                && TryParseUniversalDateTime(success.Result, out var dateTime))
            {
                return dateTime;
            }

            return _clock.GetNowDateAndTime();
        }

        public Guid GetTestCaseStartedPickleId(ScenarioInfo scenarioInfo)
        {
            if (_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseStartedPickleIdOverrideName) is ISuccess<string> success
                && Guid.TryParse(success.Result, out var pickleId))
            {
                return pickleId;
            }

            return _pickleIdStore.GetPickleIdForScenario(scenarioInfo);
        }

        public DateTime GetTestCaseFinishedTime()
        {
            if (_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseFinishedTimeOverrideName) is ISuccess<string> success
                && TryParseUniversalDateTime(success.Result, out var dateTime))
            {
                return dateTime;
            }

            return _clock.GetNowDateAndTime();
        }

        public Guid GetTestCaseFinishedPickleId(ScenarioInfo scenarioInfo)
        {
            if (_environmentWrapper.GetEnvironmentVariable(SpecFlowMessagesTestCaseFinishedPickleIdOverrideName) is ISuccess<string> success
                && Guid.TryParse(success.Result, out var pickleId))
            {
                return pickleId;
            }

            return _pickleIdStore.GetPickleIdForScenario(scenarioInfo);
        }
    }
}
