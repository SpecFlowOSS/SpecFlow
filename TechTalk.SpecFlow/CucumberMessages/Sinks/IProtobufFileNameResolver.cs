using TechTalk.SpecFlow.CommonModels;

namespace TechTalk.SpecFlow.CucumberMessages.Sinks
{
    public interface IProtobufFileNameResolver
    {
        IResult<string> Resolve(string targetPath);
    }
}
