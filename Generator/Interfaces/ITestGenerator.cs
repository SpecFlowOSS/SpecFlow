using System.IO;

namespace TechTalk.SpecFlow.Generator.Interfaces
{
    public interface ITestGenerator
    {
        TestGeneratorResult GenerateTestFile(FeatureFileInput featureFileInput, GenerationSettings settings);
    }
}
