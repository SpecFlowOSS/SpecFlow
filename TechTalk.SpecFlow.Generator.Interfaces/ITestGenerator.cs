using System;
using System.IO;
using System.Linq;

namespace TechTalk.SpecFlow.Generator
{
    public interface ITestGenerator
    {
        void GenerateTestFile(FeatureFileInput featureFileInput, TextWriter outputWriter, GenerationSettings settings);
    }
}
