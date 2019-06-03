using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages.RowObjects;

namespace TechTalk.SpecFlow.Specs.Drivers.CucumberMessages
{
    public class TestCaseFinishedDriver
    {
        private readonly CucumberMessagesDriver _cucumberMessagesDriver;

        public TestCaseFinishedDriver(CucumberMessagesDriver cucumberMessagesDriver)
        {
            _cucumberMessagesDriver = cucumberMessagesDriver;
        }

        public void TestCaseFinishedMessagesShouldHaveBeenSent(int amount)
        {
            var messageQueue = _cucumberMessagesDriver.LoadMessageQueue();
            messageQueue.ToArray().OfType<TestCaseFinished>().Should().HaveCount(amount);
        }

        public void TestCaseFinishedMessageShouldHaveBeenSent(Table values)
        {
            var messageQueue = _cucumberMessagesDriver.LoadMessageQueue();
            var testCaseFinished = messageQueue.ToArray().OfType<TestCaseFinished>().First();
            var testCaseFinishedRow = values.CreateInstance<TestCaseFinishedRow>();

            if (testCaseFinishedRow.Timestamp is string expectedTimeStampString
                && DateTime.TryParse(expectedTimeStampString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var expectedTimeStamp))
            {
                testCaseFinished.Timestamp.ToDateTime().Should().Be(expectedTimeStamp);
            }

            if (testCaseFinishedRow.PickleId is string expectedPickleId)
            {
                testCaseFinished.PickleId.Should().Be(expectedPickleId);
            }
        }
    }
}
