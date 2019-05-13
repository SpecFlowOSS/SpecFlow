using System;
using Google.Protobuf.WellKnownTypes;
using Io.Cucumber.Messages;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public class CucumberMessageFactory : ICucumberMessageFactory
    {
        private const string UsedCucumberImplementationString = @"SpecFlow";

        public TestRunStarted BuildTestRunStartedMessage(DateTime timeStamp)
        {
            return new TestRunStarted
            {
                Timestamp = Timestamp.FromDateTime(timeStamp),
                CucumberImplementation = UsedCucumberImplementationString
            };
        }
    }
}
