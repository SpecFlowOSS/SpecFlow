using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageFactory
    {
        IResult<TestRunStarted> BuildTestRunStartedMessage(DateTime timeStamp);

        IResult<TestCaseStarted> BuildTestCaseStartedMessage(Guid pickleId, DateTime timeStamp, TestCaseStarted.Types.Platform platform);

        IResult<TestCaseFinished> BuildTestCaseFinishedMessage(Guid pickleId, DateTime timeStamp, TestResult testResult);

        IResult<Envelope> BuildEnvelopeMessage(IResult<TestRunStarted> testRunStarted);

        IResult<Envelope> BuildEnvelopeMessage(IResult<TestCaseStarted> testCaseStarted);

        IResult<Envelope> BuildEnvelopeMessage(IResult<TestCaseFinished> testCaseFinished);
    }
}
