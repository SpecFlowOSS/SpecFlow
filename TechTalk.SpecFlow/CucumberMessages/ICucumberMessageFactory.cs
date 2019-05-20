using System;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface ICucumberMessageFactory
    {
        Result BuildTestRunStartedMessage(DateTime timeStamp);

        Result BuildTestCaseStartedMessage(Guid pickleId, DateTime timeStamp);
    }
}
