using System;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    public interface ITestGeneratorFactory
    {
        Version GetGeneratorVersion();
        ITestGenerator CreateGenerator(ProjectSettings projectSettings);
    }
}