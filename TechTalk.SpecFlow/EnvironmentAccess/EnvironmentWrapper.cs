using System;
using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.EnvironmentAccess
{
    public class EnvironmentWrapper : IEnvironmentWrapper
    {
        public IResult<string> ResolveEnvironmentVariables(string source)
        {
            if (source is null)
            {
                return Result<string>.Failure(new ArgumentNullException(nameof(source)));
            }

            return Result<string>.Success(Environment.ExpandEnvironmentVariables(source));
        }

        public bool IsEnvironmentVariableSet(string name)
        {
            return Environment.GetEnvironmentVariables().Contains(name);
        }

        public IResult<string> GetEnvironmentVariable(string name)
        {
            if (IsEnvironmentVariableSet(name))
            {
                return Result<string>.Success(Environment.GetEnvironmentVariable(name));
            }

            return Result<string>.Failure($"Environment variable {name} not set");
        }

        public void SetEnvironmentVariable(string name, string value)
        {
            Environment.SetEnvironmentVariable(name, value, EnvironmentVariableTarget.Process);
        }

        public string GetCurrentDirectory() => Environment.CurrentDirectory;
    }
}
