using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface IPlatformFactory
    {
        TestCaseStarted.Types.Platform BuildFromSystemInformation();
    }
}
