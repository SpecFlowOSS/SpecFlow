using System;
using FluentAssertions;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class CucumberMessageFactoryTests
    {
        [Fact(DisplayName = @"BuildTestRunResultMessage should return a TestRunResult message object")]
        public void BuildTestRunResultMessage_DateTime_ShouldReturnTestRunResultMessageObject()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var actualTestRunStartedMessage = cucumberMessageFactory.BuildTestRunStartedMessage(dateTime);

            // ASSERT
            actualTestRunStartedMessage.Should().NotBe(null);
        }

        [Fact(DisplayName = @"BuildTestRunResultMessage should return a TestRunResult message object with the specified date and time")]
        public void BuildTestRunResultMessage_DateTime_ShouldReturnTestRunResultMessageObjectWithSpecifiedDateAndTime()
        {
            // ARRANGE
            var cucumberMessageFactory = new CucumberMessageFactory();
            var dateTime = new DateTime(2019, 5, 9, 14, 27, 48, DateTimeKind.Utc);

            // ACT
            var actualTestRunStartedMessage = cucumberMessageFactory.BuildTestRunStartedMessage(dateTime);

            // ASSERT
            actualTestRunStartedMessage.Timestamp.ToDateTime().Should().Be(dateTime);
        }
    }
}
