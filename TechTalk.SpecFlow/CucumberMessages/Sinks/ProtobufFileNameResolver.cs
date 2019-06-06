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
            switch (resolveEnvironmentVariablesResult)
            {
                case ISuccess<string> success when Path.IsPathRooted(success.Result):
                    return Result<string>.Success(success.Result);

                case ISuccess<string> success:
                    string combinedPath = Path.Combine(_testRunContext.GetTestDirectory(), success.Result);
                    return Result<string>.Success(combinedPath);

                case IFailure failure:
                    return Result<string>.Failure($"Failed resolving environment variables from string '{targetPath}'", failure);

                default:
                    return Result<string>.Failure($"Failed resolving environment variables from string '{targetPath}'");
            }
        }
    }
}
