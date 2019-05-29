using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages.RowObjects;

namespace TechTalk.SpecFlow.Specs.Drivers.CucumberMessages
{
    public class TestCaseStartedDriver
    {
        private readonly CucumberMessagesDriver _cucumberMessagesDriver;

        public TestCaseStartedDriver(CucumberMessagesDriver cucumberMessagesDriver)
        {
            _cucumberMessagesDriver = cucumberMessagesDriver;
        }

        public void TestCaseStartedMessagesShouldHaveBeenSent(int amount)
        {
            var messageQueue = _cucumberMessagesDriver.LoadMessageQueue();
            messageQueue.ToArray().OfType<TestCaseStarted>().Should().HaveCount(amount);
        }

        public void TestCaseStartedMessageShouldHaveBeenSent(Table values)
        {
            var messageQueue = _cucumberMessagesDriver.LoadMessageQueue();
            var testCaseStarted = messageQueue.ToArray().OfType<TestCaseStarted>().First();
            var testCaseStartedRow = values.CreateInstance<TestCaseStartedRow>();

            if (testCaseStartedRow.Timestamp is string expectedTimeStampString
                && DateTime.TryParse(expectedTimeStampString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var expectedTimeStamp))
            {
                testCaseStarted.Timestamp.ToDateTime().Should().Be(expectedTimeStamp);
            }

            if (testCaseStartedRow.PickleId is string expectedPickleId)
            {
                testCaseStarted.PickleId.Should().Be(expectedPickleId);
            }
        }
    }
}
