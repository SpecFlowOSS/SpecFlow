using Io.Cucumber.Messages;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages
{
    public interface IPlatformFactory
    {
        IResult<TestCaseStarted.Types.Platform> BuildFromSystemInformation();
    }
}
