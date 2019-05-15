using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages.Configuration
{
    public interface ISinkConfiguration
    {
        string TypeName { get; }

        Result<string> GetString(string name);
    }
}
