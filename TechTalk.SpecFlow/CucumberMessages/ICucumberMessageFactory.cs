using System;
using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageFactory
    {
        Result BuildTestRunStartedMessage(DateTime timeStamp);

        Result BuildTestCaseStartedMessage(string pickleId, DateTime timeStamp);
    }
}
