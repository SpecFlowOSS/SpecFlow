using System;
using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageFactory
    {
        TestRunStarted BuildTestRunStartedMessage(DateTime timeStamp);
    }
}
