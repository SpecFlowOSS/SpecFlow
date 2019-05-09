using System;
using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageFactory : ICucumberMessageFactory
    {
        public TestRunStarted BuildTestRunStartedMessage(DateTime timeStamp)
        {
            return new TestRunStarted();
        }
    }
}
