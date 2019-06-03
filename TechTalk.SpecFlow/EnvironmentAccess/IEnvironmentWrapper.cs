using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.EnvironmentAccess
{
    public interface IEnvironmentWrapper
    {
        IResult<string> ResolveEnvironmentVariables(string source);

        bool IsEnvironmentVariableSet(string name);

        IResult<string> GetEnvironmentVariable(string name);

        void SetEnvironmentVariable(string name, string value);

        string GetCurrentDirectory();
    }
}
