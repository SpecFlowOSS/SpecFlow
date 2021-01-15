using System;
using System.Collections.Generic;
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
            Envelope sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Envelope>()))
                                   .Callback<Envelope>(m => sentMessage = m);

            var cucumberMessageFactoryMock = GetCucumberMessageFactoryMock();
            var fieldValueProviderMock = GetFieldValueProviderMock();
            var platformFactoryMock = GetPlatformFactoryMock();
            var testRunResultSuccessCalculatorMock = GetTestRunResultSuccessCalculatorMock();

            var sinkProviderMock = new Mock<ISinkProvider>();
            sinkProviderMock.Setup(m => m.GetMessageSinksFromConfiguration()).Returns(new List<ICucumberMessageSink>() { cucumberMessageSinkMock.Object });
            var cucumberMessageSender = new CucumberMessageSender(cucumberMessageFactoryMock.Object, platformFactoryMock.Object, fieldValueProviderMock.Object, testRunResultSuccessCalculatorMock.Object, sinkProviderMock.Object);
            var scenarioInfo = new ScenarioInfo("Test", "Description", new string[] { "Tag1" }, null);

            // ACT
            cucumberMessageSender.SendTestCaseStarted(scenarioInfo);

            // ASSERT
            sentMessage.MessageCase.Should().Be(Envelope.MessageOneofCase.TestCaseStarted);
        }

        public Mock<ITestRunResultSuccessCalculator> GetTestRunResultSuccessCalculatorMock(bool isSuccess = true)
        {
            var testRunResultSuccessCalculatorMock = new Mock<ITestRunResultSuccessCalculator>();
            testRunResultSuccessCalculatorMock.Setup(m => m.IsSuccess(It.IsAny<TestRunResult>()))
                                          .Returns(isSuccess);
            return testRunResultSuccessCalculatorMock;
        }

        public Mock<IPlatformFactory> GetPlatformFactoryMock()
        {
            var platformFactoryMock = new Mock<IPlatformFactory>();
            platformFactoryMock.Setup(m => m.BuildFromSystemInformation())
                               .Returns(
                                   new TestCaseStarted.Types.Platform
                                   {
                                       Cpu = "x64",
                                       Os = "Windows",
                                       Implementation = "SpecFlow",
                                       Version = "3.1.0"
                                   });
            return platformFactoryMock;
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStated message to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedToSink()
        {
            // ARRANGE
            Envelope sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Envelope>()))
                                   .Callback<Envelope>(m => sentMessage = m);

            var cucumberMessageFactoryMock = GetCucumberMessageFactoryMock();
            var fieldValueProviderMock = GetFieldValueProviderMock();
            var platformFactoryMock = GetPlatformFactoryMock();
            var testRunResultSuccessCalculatorMock = GetTestRunResultSuccessCalculatorMock();

            var sinkProviderMock = new Mock<ISinkProvider>();
            sinkProviderMock.Setup(m => m.GetMessageSinksFromConfiguration()).Returns(new List<ICucumberMessageSink>() { cucumberMessageSinkMock.Object });

            var cucumberMessageSender = new CucumberMessageSender(cucumberMessageFactoryMock.Object, platformFactoryMock.Object, fieldValueProviderMock.Object, testRunResultSuccessCalculatorMock.Object, sinkProviderMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Envelope.MessageOneofCase.TestRunStarted);
        }

        [Fact(DisplayName = @"SendTestRunStarted should send a TestRunStated message with correct time stamp to sink")]
        public void SendTestRunStarted_ShouldSendTestRunStartedWithCorrectTimeStampToSink()
        {
            // ARRANGE
            var now = new DateTime(2019, 5, 9, 15, 46, 5, DateTimeKind.Utc);

            Envelope sentMessage = default;

            var cucumberMessageSinkMock = new Mock<ICucumberMessageSink>();
            cucumberMessageSinkMock.Setup(m => m.SendMessage(It.IsAny<Envelope>()))
                                   .Callback<Envelope>(m => sentMessage = m);

            var cucumberMessageFactoryMock = GetCucumberMessageFactoryMock();
            var fieldValueProviderMock = GetFieldValueProviderMock(testRunStartedTimeStamp: now);
            var platformFactoryMock = GetPlatformFactoryMock();
            var testRunResultSuccessCalculatorMock = GetTestRunResultSuccessCalculatorMock();

            var sinkProviderMock = new Mock<ISinkProvider>();
            sinkProviderMock.Setup(m => m.GetMessageSinksFromConfiguration()).Returns(new List<ICucumberMessageSink>() { cucumberMessageSinkMock.Object });

            var cucumberMessageSender = new CucumberMessageSender(cucumberMessageFactoryMock.Object, platformFactoryMock.Object, fieldValueProviderMock.Object, testRunResultSuccessCalculatorMock.Object, sinkProviderMock.Object);

            // ACT
            cucumberMessageSender.SendTestRunStarted();

            // ASSERT
            sentMessage.MessageCase.Should().Be(Envelope.MessageOneofCase.TestRunStarted);
            sentMessage.TestRunStarted.Timestamp.ToDateTime().Should().Be(now);
        }

        public Mock<ICucumberMessageFactory> GetCucumberMessageFactoryMock()
        {
            var cucumberMessageFactoryMock = new Mock<ICucumberMessageFactory>();
            cucumberMessageFactoryMock.Setup(m => m.BuildEnvelopeMessage(It.IsAny<ISuccess<TestCaseStarted>>()))
                                      .Returns<ISuccess<TestCaseStarted>>(r => Result<Envelope>.Success(new Envelope { TestCaseStarted = r.Result }));

            cucumberMessageFactoryMock.Setup(m => m.BuildEnvelopeMessage(It.IsAny<ISuccess<TestRunStarted>>()))
                                      .Returns<ISuccess<TestRunStarted>>(r => Result<Envelope>.Success(new Envelope { TestRunStarted = r.Result }));

            cucumberMessageFactoryMock.Setup(m => m.BuildTestRunStartedMessage(It.IsAny<DateTime>()))
                                      .Returns<DateTime>(timeStamp => Result<TestRunStarted>.Success(
                                          new TestRunStarted
                                          {
                                              Timestamp = timeStamp.ToCucumberMessagesTimestamp()
                                          }));

            cucumberMessageFactoryMock.Setup(m => m.BuildTestCaseStartedMessage(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<TestCaseStarted.Types.Platform>()))
                                      .Returns<Guid, DateTime, TestCaseStarted.Types.Platform>((id, timeStamp, platform) => Result<TestCaseStarted>.Success(
                                          new TestCaseStarted
                                          {
                                              PickleId = $"{id:D}",
                                              Timestamp = timeStamp.ToCucumberMessagesTimestamp(),
                                              Platform = platform
                                          }));
            return cucumberMessageFactoryMock;
        }

        public Mock<IFieldValueProvider> GetFieldValueProviderMock(
            DateTime? testRunStartedTimeStamp = default,
            DateTime? testCaseStartedTimeStamp = default,
            Guid? testCaseStartedPickleId = default,
            DateTime? testCaseFinishedTimeStamp = default,
            Guid? testCaseFinishedPickleId = default)
        {
            var fieldValueProviderMock = new Mock<IFieldValueProvider>();
            fieldValueProviderMock.Setup(m => m.GetTestRunStartedTime())
                                                    .Returns(() => testRunStartedTimeStamp ?? DateTime.UtcNow);
            fieldValueProviderMock.Setup(m => m.GetTestCaseStartedTime())
                                                    .Returns(() => testCaseStartedTimeStamp ?? DateTime.UtcNow);
            fieldValueProviderMock.Setup(m => m.GetTestCaseStartedPickleId(It.IsAny<ScenarioInfo>()))
                                                    .Returns(testCaseStartedPickleId ?? Guid.NewGuid());
            fieldValueProviderMock.Setup(m => m.GetTestCaseFinishedTime())
                                                    .Returns(() => testCaseFinishedTimeStamp ?? DateTime.UtcNow);
            fieldValueProviderMock.Setup(m => m.GetTestCaseFinishedPickleId(It.IsAny<ScenarioInfo>()))
                                                    .Returns(testCaseFinishedPickleId ?? Guid.NewGuid());
            return fieldValueProviderMock;
        }
    }
}
