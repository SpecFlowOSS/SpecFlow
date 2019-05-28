using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.EnvironmentAccess
{
    public interface IEnvironmentWrapper
    {
        bool IsEnvironmentVariableSet(string name);

        IResult<string> GetEnvironmentVariable(string name);

        void SetEnvironmentVariable(string name, string value);
    }
}
