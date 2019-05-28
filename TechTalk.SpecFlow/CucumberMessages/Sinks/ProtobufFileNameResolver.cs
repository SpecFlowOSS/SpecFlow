using System.IO;
using TechTalk.SpecFlow.CommonModels;
using TechTalk.SpecFlow.EnvironmentAccess;
using TechTalk.SpecFlow.TestFramework;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public class ProtobufFileNameResolver : IProtobufFileNameResolver
    {
        private readonly ITestRunContext _testRunContext;
        private readonly IEnvironmentWrapper _environmentWrapper;

        public ProtobufFileNameResolver(ITestRunContext testRunContext, IEnvironmentWrapper environmentWrapper)
        {
            _testRunContext = testRunContext;
            _environmentWrapper = environmentWrapper;
        }

        public IResult<string> Resolve(string targetPath)
        {
            var resolveEnvironmentVariablesResult = _environmentWrapper.ResolveEnvironmentVariables(targetPath);
            if (!(resolveEnvironmentVariablesResult is ISuccess<string> success))
            {
                return Result<string>.Failure($"Failed resolving environment variables from string '{targetPath}'");
            }

            if (Path.IsPathRooted(success.Result))
            {
                return Result<string>.Success(success.Result);
            }

            string combinedPath = Path.Combine( _testRunContext.TestDirectory, targetPath);
            return Result<string>.Success(combinedPath);
        }
    }
}
