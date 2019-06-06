using System;
using FluentAssertions;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;
using Moq;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.CucumberMessages;
using Xunit;

namespace TechTalk.SpecFlow.RuntimeTests.CucumberMessages
{
    public class CucumberMessageSenderTests
    {
        [Fact(DisplayName = @"SendTestCaseStarted should send a TestCaseStarted message to sink")]
        public void SendTestCaseStarted_ValidParameters_ShouldSendTestRunStartedToSink()
        {
            // ARRANGE
            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageFactoryMock = GetCucumberMessageFactoryMock();
            var cucumberMessageSenderValueMockSourceMock = GetCucumberMessageSenderValueMockSourceMock();

            var cucumberMessageSender = new CucumberMessageSender(cucumberMessageFactoryMock.Object, cucumberMessageSinkMock.Object, cucumberMessageSenderValueMockSourceMock.Object);
            var scenarioInfo = new ScenarioInfo("Test", "Description", "Tag1");

            // ACT
            cucumberMessageSender.SendTestCaseStarted(scenarioInfo);

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestCaseStarted);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStated message to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedToSink()
        {
            // ARRANGE
            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageFactoryMock = GetCucumberMessageFactoryMock();
            var cucumberMessageSenderValueMockSourceMock = GetCucumberMessageSenderValueMockSourceMock();

            var cucumberMessageSender = new CucumberMessageSender(cucumberMessageFactoryMock.Object, cucumberMessageSinkMock.Object, cucumberMessageSenderValueMockSourceMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestRunStarted);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStated message with correct time stamp to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedWithCorrectTimeStampToSink()
        {
            // ARRANGE
            var now = new DateTime(2019, 5, 9, 15, 46, 5, DateTimeKind.Utc);

            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageFactoryMock = GetCucumberMessageFactoryMock();
            var cucumberMessageSenderValueMockSourceMock = GetCucumberMessageSenderValueMockSourceMock(testRunStartedTimeStamp: now);

            var cucumberMessageSender = new CucumberMessageSender(cucumberMessageFactoryMock.Object, cucumberMessageSinkMock.Object, cucumberMessageSenderValueMockSourceMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestRunStarted);
            sentMessage.TestRunStarted.Timestamp.ToDateTime().Should().Be(now);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStarted message with SpecFlow as used Cucumber implementation to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedWithSpecFlowAsUsedCucumberImplementationToSink()
        {
            // ARRANGE
            const string expectedCucumberImplementation = @"SpecFlow";
            Wrapper sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Wrapper>()))
                                   .Callback<Wrapper>(m => sentMessage = m);

            var cucumberMessageFactoryMock = GetCucumberMessageFactoryMock();
            var cucumberMessageSenderValueMockSourceMock = GetCucumberMessageSenderValueMockSourceMock();

            var cucumberMessageSender = new CucumberMessageSender(cucumberMessageFactoryMock.Object, cucumberMessageSinkMock.Object, cucumberMessageSenderValueMockSourceMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Wrapper.MessageOneofCase.TestRunStarted);
            sentMessage.TestRunStarted.CucumberImplementation.Should().Be(expectedCucumberImplementation);
        }

        public Mock<ICucumberMessageFactory> GetCucumberMessageFactoryMock(string cucumberImplementation = "SpecFlow")
        {
            var cucumberMessageFactoryMock = new Mock<ICucumberMessageFactory>();
            cucumberMessageFactoryMock.Setup(m => m.BuildWrapperMessage(It.IsAny<ISuccess<TestCaseStarted>>()))
                                      .Returns<ISuccess<TestCaseStarted>>(r => Result<Wrapper>.Success(new Wrapper { TestCaseStarted = r.Result }));

            cucumberMessageFactoryMock.Setup(m => m.BuildWrapperMessage(It.IsAny<ISuccess<TestRunStarted>>()))
                                      .Returns<ISuccess<TestRunStarted>>(r => Result<Wrapper>.Success(new Wrapper { TestRunStarted = r.Result }));

            cucumberMessageFactoryMock.Setup(m => m.BuildTestRunStartedMessage(It.IsAny<DateTime>()))
                                      .Returns<DateTime>(timeStamp => Result<TestRunStarted>.Success(
                                          new TestRunStarted
                                          {
                                              Timestamp = Timestamp.FromDateTime(timeStamp),
                                              CucumberImplementation = cucumberImplementation
                                          }));

            cucumberMessageFactoryMock.Setup(m => m.BuildTestCaseStartedMessage(It.IsAny<Guid>(), It.IsAny<DateTime>()))
                                      .Returns<Guid, DateTime>((id, timeStamp) => Result<TestCaseStarted>.Success(
                                          new TestCaseStarted
                                          {
                                              PickleId = $"{id:D}",
                                              Timestamp = Timestamp.FromDateTime(timeStamp)
                                          }));
            return cucumberMessageFactoryMock;
        }

        public Mock<ICucumberMessageSenderValueMockSource> GetCucumberMessageSenderValueMockSourceMock(
            DateTime? testRunStartedTimeStamp = default,
            DateTime? testCaseStartedTimeStamp = default,
            Guid? testCaseStartedPickleId = default,
            DateTime? testCaseFinishedTimeStamp = default,
            Guid? testCaseFinishedPickleId = default)
        {
            var cucumberMessageSenderValueMockSourceMock = new Mock<ICucumberMessageSenderValueMockSource>();
            cucumberMessageSenderValueMockSourceMock.Setup(m => m.GetTestRunStartedTime())
                                                    .Returns(() => testRunStartedTimeStamp ?? DateTime.UtcNow);
            cucumberMessageSenderValueMockSourceMock.Setup(m => m.GetTestCaseStartedTime())
                                                    .Returns(() => testCaseStartedTimeStamp ?? DateTime.UtcNow);
            cucumberMessageSenderValueMockSourceMock.Setup(m => m.GetTestCaseStartedPickleId(It.IsAny<ScenarioInfo>()))
                                                    .Returns(testCaseStartedPickleId ?? Guid.NewGuid());
            cucumberMessageSenderValueMockSourceMock.Setup(m => m.GetTestCaseFinishedTime())
                                                    .Returns(() => testCaseFinishedTimeStamp ?? DateTime.UtcNow);
            cucumberMessageSenderValueMockSourceMock.Setup(m => m.GetTestCaseFinishedPickleId(It.IsAny<ScenarioInfo>()))
                                                    .Returns(testCaseFinishedPickleId ?? Guid.NewGuid());
            return cucumberMessageSenderValueMockSourceMock;
        }
    }
}
