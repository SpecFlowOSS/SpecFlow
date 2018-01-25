using System;
using System.IO;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    /// IMPORTANT
    /// This class is used for interop with the Visual Studio Extension
    /// DO NOT REMOVE OR RENAME FIELDS!
    /// This breaks binary serialization accross appdomains
    public interface ITestGenerator : IDisposable
    {
        TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings);
        Version DetectGeneratedTestVersion(FeatureFileInput featureFileInput);
        string GetTestFullPath(FeatureFileInput featureFileInput);
    }
}
