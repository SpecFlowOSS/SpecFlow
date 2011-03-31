using TechTalk.SpecFlow.Generator.Configuration;

namespace TechTalk.SpecFlow.Generator.Project
{
    public class AppDomainProjectReference : IProjectReference
    {
        static public AppDomainProjectReference AssertFileProjectReference(IProjectReference projectReference)
        {
            return (AppDomainProjectReference)projectReference; //TODO: better error handling
        }
    }
}