using System;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.Assist;
using TechTalk.SpecFlow.Specs.Drivers.CucumberMessages.RowObjects;

namespace TechTalk.SpecFlow.Specs.Drivers.CucumberMessages
{
    public class TestRunStartedDriver
    {
        private readonly CucumberMessagesDriver _cucumberMessagesDriver;

        public TestRunStartedDriver(CucumberMessagesDriver cucumberMessagesDriver)
        {
            _cucumberMessagesDriver = cucumberMessagesDriver;
        }

        public void TestRunStartedMessageShouldHaveBeenSent()
        {
            var messageQueue = _cucumberMessagesDriver.LoadMessageQueue();
            messageQueue.ToArray().Should().Contain(m => m is TestRunStarted);
        }

        public void TestRunStartedMessageShouldHaveBeenSent(int amount)
        {
            var messageQueue = _cucumberMessagesDriver.LoadMessageQueue();
            messageQueue.ToArray().OfType<TestRunStarted>().Should().HaveCount(amount);
        }

        public void TestRunStartedMessageShouldHaveBeenSent(Table values)
        {
            var messageQueue = _cucumberMessagesDriver.LoadMessageQueue();
            var testRunStarted = messageQueue.ToArray().Should().Contain(m => m is TestRunStarted)
                                             .Which.Should().BeOfType<TestRunStarted>()
                                             .Which;
            var testRunStartedRow = values.CreateInstance<TestRunStartedRow>();

            if (testRunStartedRow.Timestamp is string expectedTimeStampString
                && DateTime.TryParse(expectedTimeStampString, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal, out var expectedTimeStamp))
            {
                testRunStarted.Timestamp.ToDateTime().Should().Be(expectedTimeStamp);
            }

            if (testRunStartedRow.CucumberImplementation is string expectedCucumberImplementation)
            {
                testRunStarted.CucumberImplementation.Should().Be(expectedCucumberImplementation);
            }
        }
    }
}
